using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Xml.Linq;
using WebApp.Entities.Models;

namespace WebApp.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Genre Genre { get; set; }

        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTime ReleaseDate { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        [Range(0.0, 10.0)]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public decimal Rating { get; set; }
        public int Runtime { get; set; }
        public string Plot { get; set; }
        public string PosterURL { get; set; } = "";

        [NotMapped]
        public IEnumerable<SelectListItem> Genres { get; set; } = new List<SelectListItem>();

        [NotMapped]
        [DisplayName("Upload Poster Image")]
        public IFormFile? PosterFile { get; set; }
    }
}
