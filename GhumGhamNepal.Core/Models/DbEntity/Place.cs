using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhumGhamNepal.Core.Models.DbEntity
{
    public partial class Place
    {
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string Location { get; set; }
        public string Introduction { get; set; }
        public string Description1 { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FileFormat { get; set; }
        [StringLength(50)]
        public string Size { get; set; }
        [StringLength(150)]
        public string FileType { get; set; }
        [StringLength(150)]
        public string ServerFileName { get; set; }
        [StringLength(150)]
        public string UserFileName { get; set; }
        [StringLength(150)]
        public string FileLocation { get; set; }
    }
}
