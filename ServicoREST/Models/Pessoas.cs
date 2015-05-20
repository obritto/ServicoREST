namespace ServicoREST.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Pessoas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idPessoa { get; set; }

        [Required]
        [StringLength(50)]
        public string nome { get; set; }

        [Required]
        [StringLength(50)]
        public string telefone { get; set; }

        public bool? ativo { get; set; }
    }
}
