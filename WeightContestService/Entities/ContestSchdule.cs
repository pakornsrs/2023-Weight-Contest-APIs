using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeightContestService.Entities
{
    [Table("CONTEST_SCHDULE")]
    public class ContestSchdule
    {
        public int ID { get; set; }
        public DateTime SCHDULE_DATA { get; set; }
        public DateTime START_SUBMIT_DATE { get; set; }
        public DateTime END_SUBMIT_DATE { get; set; }
        public int PHASE { get; set; }
    }
}
