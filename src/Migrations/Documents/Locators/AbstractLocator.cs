namespace AlbedoTeam.Sdk.DataLayerAccess.Migrations.Documents.Locators
{
    using System.Collections.Generic;

    internal abstract class AbstractLocator<TReturnType, TTypeIdentifier> : ILocator<TReturnType, TTypeIdentifier>
        where TReturnType : struct
        where TTypeIdentifier : class
    {
        private IDictionary<TTypeIdentifier, TReturnType> _locatesDictionary;

        protected IDictionary<TTypeIdentifier, TReturnType> LocatesDictionary
        {
            get
            {
                if (_locatesDictionary == null)
                    Locate();
                return _locatesDictionary;
            }

            set => _locatesDictionary = value;
        }

        public abstract TReturnType? GetLocateOrNull(TTypeIdentifier identifier);
        public abstract void Locate();
    }
}