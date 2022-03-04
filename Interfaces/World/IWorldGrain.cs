using Base;
using Orleans;

namespace Interfaces.World;

public interface IWorldGrain : IGrainWithIntegerKey, IBaseGrain
{
}