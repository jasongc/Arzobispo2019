﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Common
{
    public class MultimediaFileBE
    {
        [Key]
        public string MultimediaFileId { get; set; }
        public string PersonId { get; set; }
        public string FileName { get; set; }
        public byte [] File { get; set; }
        public byte [] ThumbnailFile { get; set; }
        public string Comment { get; set; }
        public int? IsDeleted { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? UpdateUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
