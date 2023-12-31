﻿namespace PropertiesAPI.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ArchiveId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }
        public bool Deleted { get; set; }
        public int Version { get; set; } = 0;




    }
}
