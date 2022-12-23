using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Models
{
    public class UserModel
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
