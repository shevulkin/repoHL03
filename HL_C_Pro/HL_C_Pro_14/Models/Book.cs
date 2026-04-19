using System;
using System.Collections;
using System.Collections.Generic;

namespace HL_C_Pro_14.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Price { get; set; }

    public int Categoryid { get; set; }

    public BitArray? Isdeleted { get; set; }

    public virtual Category Category { get; set; } = null!;
}
