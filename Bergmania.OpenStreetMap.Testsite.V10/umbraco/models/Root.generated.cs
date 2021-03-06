//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	/// <summary>Root</summary>
	[PublishedModel("root")]
	public partial class Root : PublishedContentModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		public new const string ModelTypeAlias = "root";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<Root, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public Root(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Map 1
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("map1")]
		public virtual global::Bergmania.OpenStreetMap.Core.OpenStreetMapModel Map1 => this.Value<global::Bergmania.OpenStreetMap.Core.OpenStreetMapModel>(_publishedValueFallback, "map1");

		///<summary>
		/// Map 2
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "10.0.0-preview20220502.91295+f9840ed4b155b1e51ea9c3c5b20f34c1cee12335")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("map2")]
		public virtual global::Bergmania.OpenStreetMap.Core.OpenStreetMapModel Map2 => this.Value<global::Bergmania.OpenStreetMap.Core.OpenStreetMapModel>(_publishedValueFallback, "map2");
	}
}
