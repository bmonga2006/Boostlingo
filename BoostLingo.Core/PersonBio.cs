
using System.ComponentModel.DataAnnotations.Schema;


namespace BoostLingo.Core
{
    public class PersonBio
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonBioId { get; set; }
        public int PersonId { get; set; }
        public string BioText { get; set; }
        public Person Person { get; set; }

    }
}
