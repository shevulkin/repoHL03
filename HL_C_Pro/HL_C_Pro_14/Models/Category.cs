using System;
using System.Collections.Generic;

namespace HL_C_Pro_14.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Categoryname { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
