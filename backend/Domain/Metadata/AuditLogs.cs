using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Domain.Metadata
{
    [Table("AuditLogs", Schema = "public")]
    public class AuditLog
    {

        public int Id { get; set; }

        [Column("AuditData", TypeName = "jsonb")]
        public string AuditData { get; set; } = string.Empty; // Store as stringified JSON

        [Column("EventDate")]
        public DateTime EventDate { get; set; }

        [Column("User")]
        [StringLength(255)]
        public string? User { get; set; }

        [Column("Action")]
        [StringLength(255)]
        public string? Action { get; set; }
    }
}