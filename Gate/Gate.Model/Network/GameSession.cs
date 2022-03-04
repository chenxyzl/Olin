using Base;
using Base.Helper;
using Base.Serialize;
using DotNetty.Buffers;
using Interfaces.Gate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using DotNetty.Transport.Channels;
using Interfaces.Home;
using Interfaces.Login;
using Message;
using IByteBuffer = MongoDB.Bson.IO.IByteBuffer;

namespace Gate.Model.Network
{
    public class GameSession
    {
        private readonly IClusterClient _client;

        private readonly ILogger _logger;

        //private readonly Guid _sessionId;
        private readonly IConfiguration _configuration;

        private OutcomingPacketObserver _packetObserver;
        private IPacketObserver _packetObserverRef;
        private IChannelHandlerContext _context;
        private IPlayerGrain _user;
        private IAccountGrain _accountGrain;
        private bool _isInit;

        private long _userId;

        //private string _token;
        private string _md5Key;

        public GameSession(IClusterClient client, ILoggerFactory loggerFactory,
            IConfiguration configuration, IChannelHandlerContext context)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<GameSession>();
            _configuration = configuration;
            _context = context;
            //_sessionId = Guid.NewGuid();
            _md5Key = _configuration.GetValue<string>("MD5Key");
        }

        public async Task Disconnect()
        {
            await _user.UnbindPacketObserver();
        }

        bool CheckSign(GRequest packet)
        {
            //todo 签名验证
            return true;
        }

        public async Task DispatchIncomingPacket(GRequest packet)
        {
            try
            {
                //先验证token--避免被攻击
                A.Ensure(CheckSign(packet), des: "first message must login", serious: true);
                //同步初始化
                if (!_isInit)
                {
                    //第一条消息必须是登录--为了避免被攻击，直接断开
                    A.Ensure(packet.Opcode == (int) OuterOpCode.Login, Code.Error, des: "first message must login",
                        serious: true);

                    //处理登录
                    _userId = packet.UserId;
                    //_token = tokenInfo.Token;
                    _packetObserver = new OutcomingPacketObserver(this);
                    _user = _client.GetGrain<IPlayerGrain>(_userId);
                    _packetObserverRef = _client.CreateObjectReference<IPacketObserver>(_packetObserver).Result;
                    _user.BindPacketObserver(_packetObserverRef).Wait();
                    _isInit = true;
                    return;
                }

                //心跳
                if (packet.Opcode == (uint) OuterOpCode.Ping)
                {
                    var ret = new GResponse
                    {
                        Opcode = packet.Opcode, Sn = packet.Sn, Code = Code.Ok,
                        Content = new S2CPong {Time = TimeHelper.Now()}.ToBinary()
                    };
                    await DispatchOutcomingPacket(ret);
                    return;
                }

                //if (_userId != packet.UserId || _token != packet.Token)
                //{
                //    await DispatchOutcomingPacket(packet.ParseResult(MOErrorType.Hidden, "Token验证失败"));
                //    await Close();
                //    return;
                //}
                var tokenInfo = _accountGrain.GetToken().Result;
                if (tokenInfo.Token != packet.Token ||
                    tokenInfo.LastTime.AddSeconds(GameConstants.TOKENEXPIRE) < DateTime.Now)
                {
                    await DispatchOutcomingPacket(packet.ParseResult(MOErrorType.Hidden, "Token验证失败"));
                    await Close();
                    return;
                }

                await _router.SendPacket(packet);

                //watch.Stop();
                //Console.WriteLine($"{packet.UserId} {watch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "DispatchIncomingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task DispatchOutcomingPacket(GResponse packet)
        {
            try
            {
                if (_context.Channel.Active)
                {
                    var bytes = packet.ToBinary();
                    await _context.WriteAndFlushAsync(bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "DispatchOutComingPacket异常:\n" +
                    $"{ex.Message}\n" +
                    $"{ex.StackTrace}\n" +
                    $"{JsonConvert.SerializeObject(packet)}");
            }
        }

        public async Task Close()
        {
            await _context.CloseAsync();
        }

        class OutcomingPacketObserver : IPacketObserver
        {
            private readonly GameSession session;

            public OutcomingPacketObserver(GameSession session)
            {
                this.session = session;
            }

            public async void Close(MOMsg packet = null)
            {
                if (packet != null)
                    await session.DispatchOutcomingPacket(packet);
                await session.Close();
            }

            public async void SendPacket(MOMsg packet)
            {
                await session.DispatchOutcomingPacket(packet);
            }
        }
    }
}