﻿using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Bergmania.OpenStreetMap.Core
{
    public class OpenStreetMapComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddOpenStreetMap();
        }
    }
}