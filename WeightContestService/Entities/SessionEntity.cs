using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightContestService.Entities
{
    [Table("SESSION")]
    public class SessionEntity
    {
        [Key]
        public int ID { get; set; }
        public string USER_ID { get; set; }
        public DateTime START_SESSION { get; set; }
        public DateTime END_SESSION { get; set; }
        public string IP_ADDRESS { get; set; }
    }
}
