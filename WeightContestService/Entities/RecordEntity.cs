using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightContestService.Entities
{
    [Table("WEIGHT_RECORDS")]
    public class RecordEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Key]
        public int ID { get; set; } 
        public string OWNER_ID { get; set; }
        public string NAME { get; set; }
        public decimal WEIGHT { get; set; }
        public DateTime SUBMIT_DATE { get; set; }
        public DateTime? SUBMIT_SCHEDULE { get; set; }
        public string? IMAGE_PATH { get; set; }
        public bool IS_ACTIVE { get; set; } 
    }
}
