﻿namespace MS.Katusha.Infrastructure.Exceptions.Services
{
    public class KatushaFriendlyNameNotFoundException : KatushaException
    {
        public KatushaFriendlyNameNotFoundException(string guidOrFriendlyName)
            : base("FriendlyNameNotFound", "Friendly name not found.", null)
        {
            GuidOrFriendlyName = guidOrFriendlyName;
        }

        public string GuidOrFriendlyName { get; private set; }
    }

}
