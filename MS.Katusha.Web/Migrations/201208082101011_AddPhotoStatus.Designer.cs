// <auto-generated />
namespace MS.Katusha.Web.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class AddPhotoStatus : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201208082101011_AddPhotoStatus"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "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"; }
        }
    }
}