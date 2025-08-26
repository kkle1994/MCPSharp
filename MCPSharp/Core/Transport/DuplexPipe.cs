using System.IO.Pipelines;
using System.Net.Http;
using System.Net;
using System.Text;
using MCPSharp.Core.Transport.SSE;

namespace MCPSharp.Core.Transport
{
    internal class DebugStream : Stream
    {
#if DEBUG
        public static bool IsLogging = true;
#else
        public static bool IsLogging = false;
#endif
        private readonly Stream _baseStream;
        private readonly byte[] _buffer = new byte[4096];
        private readonly List<byte> _readBuffer = new();
        private static readonly string _logFilePath = Path.Combine(Path.GetTempPath(), "MCPSharp_Reader.log");
        private static readonly object _logLock = new object();

        public DebugStream(Stream baseStream)
        {
            _baseStream = baseStream;
            LogContent($"INIT: FilteringStream created for {baseStream.GetType().Name}");
        }

        private static void LogContent(string content)
        {
            if (IsLogging)
            {
                try
                {
                    lock (_logLock)
                    {
                        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {content}\n";
                        File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
                    }
                }
                catch
                {
                    // 忽略日志写入错误，避免影响主要功能
                }
            }
        }

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => _baseStream.CanWrite;
        public override long Length => _baseStream.Length;
        public override long Position { get => _baseStream.Position; set => _baseStream.Position = value; }

        public override void Flush()
        {
            LogContent("FLUSH: Flushing stream");
            _baseStream.Flush();
            LogContent("FLUSH: Flush completed");
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            LogContent("FLUSH_ASYNC: Starting async flush");
            await _baseStream.FlushAsync(cancellationToken);
            LogContent("FLUSH_ASYNC: Async flush completed");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Read from base stream into internal buffer
            int bytesRead = _baseStream.Read(_buffer, 0, _buffer.Length);
            if (bytesRead == 0) return 0;

            // Add to read buffer
            _readBuffer.AddRange(_buffer.Take(bytesRead));

            // Convert to string and log the original content
            string originalContent = Encoding.UTF8.GetString(_readBuffer.ToArray());
            LogContent($"READ: {originalContent}");

            // Convert back to bytes
            byte[] filteredBytes = Encoding.UTF8.GetBytes(originalContent);
            _readBuffer.Clear();

            // Copy to output buffer
            int copyCount = Math.Min(count, filteredBytes.Length);
            Array.Copy(filteredBytes, 0, buffer, offset, copyCount);

            // Keep remaining bytes for next read
            if (filteredBytes.Length > copyCount)
            {
                _readBuffer.AddRange(filteredBytes.Skip(copyCount));
            }

            return copyCount;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Read from base stream into internal buffer asynchronously
            byte[] tempBuffer = new byte[_buffer.Length];
            int bytesRead = await _baseStream.ReadAsync(tempBuffer, 0, tempBuffer.Length, cancellationToken);
            if (bytesRead == 0) return 0;

            // Add to read buffer
            _readBuffer.AddRange(tempBuffer.Take(bytesRead));

            // Convert to string and log the original content
            string originalContent = Encoding.UTF8.GetString(_readBuffer.ToArray());
            LogContent($"READ_ASYNC: {originalContent}");

            // Convert back to bytes
            byte[] filteredBytes = Encoding.UTF8.GetBytes(originalContent);
            _readBuffer.Clear();

            // Copy to output buffer
            int copyCount = Math.Min(count, filteredBytes.Length);
            Array.Copy(filteredBytes, 0, buffer, offset, copyCount);

            // Keep remaining bytes for next read
            if (filteredBytes.Length > copyCount)
            {
                _readBuffer.AddRange(filteredBytes.Skip(copyCount));
            }
            
            return copyCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            LogContent($"SEEK: Seeking to offset {offset} from {origin}");
            long newPosition = _baseStream.Seek(offset, origin);
            LogContent($"SEEK: New position is {newPosition}");
            return newPosition;
        }

        public override void SetLength(long value)
        {
            LogContent($"SET_LENGTH: Setting stream length to {value}");
            _baseStream.SetLength(value);
            LogContent("SET_LENGTH: Stream length set");
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Log the content being written
            string writeContent = Encoding.UTF8.GetString(buffer, offset, count);
            LogContent($"WRITE: {writeContent}");
            
            // Write to the base stream
            _baseStream.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Log the content being written asynchronously
            string writeContent = Encoding.UTF8.GetString(buffer, offset, count);
            LogContent($"WRITE_ASYNC: {writeContent}");
            
            // Write to the base stream asynchronously
            await _baseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            // Log the single byte being written
            LogContent($"WRITE_BYTE: {value:X2} ('{(char)value}')");
            
            // Write to the base stream
            _baseStream.WriteByte(value);
        }

        public override int ReadByte()
        {
            // Read from base stream
            int byteValue = _baseStream.ReadByte();
            
            // Log the single byte being read
            if (byteValue != -1)
            {
                LogContent($"READ_BYTE: {byteValue:X2} ('{(char)byteValue}')");
            }
            else
            {
                LogContent("READ_BYTE: End of stream (-1)");
            }
            
            return byteValue;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            LogContent($"BEGIN_READ: Starting async read of {count} bytes at offset {offset}");
            return _baseStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int bytesRead = _baseStream.EndRead(asyncResult);
            LogContent($"END_READ: Completed async read of {bytesRead} bytes");
            return bytesRead;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            // Log the content being written asynchronously via APM
            string writeContent = Encoding.UTF8.GetString(buffer, offset, count);
            LogContent($"BEGIN_WRITE: Starting async write - {writeContent}");
            
            return _baseStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _baseStream.EndWrite(asyncResult);
            LogContent("END_WRITE: Completed async write");
        }

        // Note: CopyTo methods are not virtual, so we cannot override them.
        // However, they will call Read/Write methods internally, which we do log.

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            LogContent($"COPY_TO_ASYNC: Starting async copy to {destination.GetType().Name} with buffer size {bufferSize}");
            await _baseStream.CopyToAsync(destination, bufferSize, cancellationToken);
            LogContent("COPY_TO_ASYNC: Async copy completed");
        }

#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            LogContent($"READ_ASYNC_MEMORY: Reading up to {buffer.Length} bytes into Memory<byte>");
            
            // For simplicity, we'll convert to byte array and use existing logic
            byte[] tempArray = new byte[buffer.Length];
            int bytesRead = await ReadAsync(tempArray, 0, buffer.Length, cancellationToken);
            
            // Copy result to Memory<byte>
            if (bytesRead > 0)
            {
                tempArray.AsMemory(0, bytesRead).CopyTo(buffer);
            }
            
            LogContent($"READ_ASYNC_MEMORY: Read {bytesRead} bytes");
            return bytesRead;
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            LogContent($"WRITE_ASYNC_MEMORY: Writing {buffer.Length} bytes from ReadOnlyMemory<byte>");
            
            // Convert to string for logging
            string writeContent = Encoding.UTF8.GetString(buffer.Span);
            LogContent($"WRITE_ASYNC_MEMORY_CONTENT: {writeContent}");
            
            // Write to base stream
            await _baseStream.WriteAsync(buffer, cancellationToken);
            
            LogContent($"WRITE_ASYNC_MEMORY: Completed writing {buffer.Length} bytes");
        }
#endif

        public static string GetLogFilePath() => _logFilePath;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LogContent("DISPOSE: FilteringStream disposed");
                _baseStream?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    internal class DuplexPipe(Stream reader, Stream writer) : IDuplexPipe
    {
#if DEBUG
        private readonly PipeReader _reader = PipeReader.Create(new DebugStream(reader));
        private readonly PipeWriter _writer = PipeWriter.Create(new DebugStream(writer));
#else
        private readonly PipeReader _reader = PipeReader.Create(reader);
        private readonly PipeWriter _writer = PipeWriter.Create(writer);
#endif

        public PipeReader Input => _reader;
        public PipeWriter Output => _writer;
    }

    internal class StdioTransportPipe : IDuplexPipe
    {
        private readonly PipeReader _reader = PipeReader.Create(Console.OpenStandardInput());
        private readonly PipeWriter _writer = PipeWriter.Create(Console.OpenStandardOutput());

        public PipeReader Input => _reader;
        public PipeWriter Output => _writer;
    }

    internal class SSETransportPipe : IDuplexPipe
    {
        private readonly HttpClient _httpClient = new();
        private readonly Uri _address;
        public SSETransportPipe(Uri address)
        {
            _address = address;
            _reader = PipeReader.Create(_httpClient.GetStreamAsync(_address).Result);
            _writer = PipeWriter.Create(new HttpPostStream(_address.ToString()));
        }

        private PipeReader _reader;
        private PipeWriter _writer;
        public PipeReader Input => _reader;
        public PipeWriter Output => _writer;
    }
}