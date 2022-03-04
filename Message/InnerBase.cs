using System.Collections.Generic;
using ProtoBuf;
namespace Message;
//服务器内部请求
[ProtoContract]
public partial class SRequest: IRequest
{
	[ProtoMember(1)]
	public uint Opcode { get; set; }

	[ProtoMember(2)]
	public byte[] Content { get; set; }

}

//服务器内部返回
[ProtoContract]
public partial class SResponse: IResponse
{
	[ProtoMember(1)]
	public uint Opcode { get; set; }

	[ProtoMember(2)]
	public byte[] Content { get; set; }

	[ProtoMember(3)]
	public Code Code { get; set; }

}

