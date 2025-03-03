using Ecommerce.Shared.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Shared.Entities;

public class Language
    {

    [Key]
    public int  Id { get; set; }

    [Required, MaxLength(10)]
    public string LanguageCode { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string LanguageName { get; set; } = string.Empty;

    }

