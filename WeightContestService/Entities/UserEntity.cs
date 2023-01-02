using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightContestService.Entities
{
    [Table("USER_DATA")]
    public class UserEntity
    {
        [Key]
        public string ID { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string NAME { get; set; }
        public decimal INIT_WEIGHT { get; set; }
        public decimal TARGET_WEIGHT { get; set; }
        public bool IS_ACCEPT_CONDITION { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
    }
}
