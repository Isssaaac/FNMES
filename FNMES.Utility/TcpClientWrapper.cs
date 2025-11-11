//using System;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

///// <summary>
///// TCP客户端封装（带自动重连）
///// </summary>
//public class TcpClientWrapper
//{
//    private readonly string _serverIp;
//    private readonly int _serverPort;
//    private TcpClient _client;
//    private NetworkStream _stream;
//    private bool _isRunning; // 客户端是否运行中
//    private bool _isConnected; // 是否已连接
//    private int _reconnectInterval = 3000; // 重连间隔（毫秒）
//    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

//    // 事件：接收消息
//    public event Action<string> OnMessageReceived;
//    // 事件：连接状态变化
//    public event Action<bool> OnConnectionStatusChanged;
//    // 事件：错误发生
//    public event Action<Exception> OnErrorOccurred;

//    /// <summary>
//    /// 初始化客户端
//    /// </summary>
//    public TcpClientWrapper(string serverIp, int serverPort)
//    {
//        _serverIp = serverIp;
//        _serverPort = serverPort;
//    }

//    /// <summary>
//    /// 启动客户端（开始连接/重连）
//    /// </summary>
//    public void Start()
//    {
//        _isRunning = true;
//        _ = StartConnectionLoop(); // 启动连接循环（异步）
//    }

//    /// <summary>
//    /// 停止客户端（断开连接并停止重连）
//    /// </summary>
//    public void Stop()
//    {
//        _isRunning = false;
//        _cts.Cancel();
//        Disconnect();
//    }

//    /// <summary>
//    /// 发送消息到服务端
//    /// </summary>
//    public async Task SendMessageAsync(string message)
//    {
//        if (!_isConnected || _stream == null)
//        {
//            OnErrorOccurred?.Invoke(new InvalidOperationException("未连接到服务端，无法发送消息"));
//            return;
//        }

//        try
//        {
//            byte[] data = Encoding.UTF8.GetBytes(message);
//            await _stream.WriteAsync(data, 0, data.Length, _cts.Token);
//            await _stream.FlushAsync(_cts.Token);
//        }
//        catch (Exception ex)
//        {
//            OnErrorOccurred?.Invoke(ex);
//            Disconnect(); // 发送失败时主动断开，触发重连
//        }
//    }

//    /// <summary>
//    /// 连接循环（自动重连逻辑）
//    /// </summary>
//    private async Task StartConnectionLoop()
//    {
//        while (_isRunning && !_cts.Token.IsCancellationRequested)
//        {
//            if (!_isConnected)
//            {
//                try
//                {
//                    Console.WriteLine($"尝试连接到 {_serverIp}:{_serverPort}...");
//                    _client = new TcpClient();
//                    await _client.ConnectAsync(_serverIp, _serverPort, _cts.Token);
//                    _stream = _client.GetStream();
//                    _isConnected = true;
//                    OnConnectionStatusChanged?.Invoke(true);
//                    Console.WriteLine("连接成功！");

//                    // 启动消息接收循环
//                    _ = StartReceiveLoop();
//                }
//                catch (Exception ex) when (!(ex is OperationCanceledException))
//                {
//                    OnErrorOccurred?.Invoke(ex);
//                    Console.WriteLine($"连接失败，{_reconnectInterval}ms后重试...");
//                    await Task.Delay(_reconnectInterval, _cts.Token);
//                }
//            }

//            await Task.Delay(100, _cts.Token); // 避免CPU空转
//        }
//    }

//    /// <summary>
//    /// 消息接收循环
//    /// </summary>
//    private async Task StartReceiveLoop()
//    {
//        byte[] buffer = new byte[1024];
//        while (_isConnected && !_cts.Token.IsCancellationRequested)
//        {
//            try
//            {
//                // 检测流是否可读，超时1秒（用于断线检测）
//                if (!await _stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token).WaitAsync(
//                    TimeSpan.FromSeconds(1), _cts.Token))
//                {
//                    continue; // 超时无数据，继续等待
//                }

//                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, _cts.Token);
//                if (bytesRead <= 0)
//                {
//                    // 读取到0字节表示连接已断开
//                    Console.WriteLine("服务端主动断开连接");
//                    Disconnect();
//                    break;
//                }

//                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//                OnMessageReceived?.Invoke(message);
//            }
//            catch (Exception ex) when (!(ex is OperationCanceledException))
//            {
//                OnErrorOccurred?.Invoke(ex);
//                Disconnect(); // 接收失败时断开连接
//                break;
//            }
//        }
//    }

//    /// <summary>
//    /// 断开连接
//    /// </summary>
//    private void Disconnect()
//    {
//        if (!_isConnected) return;

//        _isConnected = false;
//        _stream?.Dispose();
//        _client?.Close();
//        OnConnectionStatusChanged?.Invoke(false);
//        Console.WriteLine("已断开连接");
//    }
//}