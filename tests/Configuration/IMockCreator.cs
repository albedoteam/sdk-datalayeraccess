using System;
using Moq;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    public interface IMockCreator<T>
        where T : class
    {
        Mock<T> Create(params Guid[] mockedIds);
    }
}