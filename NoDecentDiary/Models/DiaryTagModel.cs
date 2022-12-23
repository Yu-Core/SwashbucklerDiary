using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Models
{
    public class DiaryTagModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int DiaryId { get; set; }
        public int TagId { get; set; }
    }
}
