using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QRCoder.PayloadGenerator;

namespace FinalProjectReservationSystems.Areas.ViewModel.Setting
{
    public class SettingViewModel
    {
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string YoutubeUrl { get; set; }

        [StringLength(maximumLength: 25)]
        public string SupportPhone { get; set; }

        [StringLength(maximumLength: 35)]
        public string SupportEmail { get; set; }

        [StringLength(maximumLength: 60)]
        public string Address { get; set; }

        [StringLength(maximumLength: 100)]
        public string FooterText { get; set; }

        [StringLength(maximumLength: 35)]
        public string FAX { get; set; }

    }
}
