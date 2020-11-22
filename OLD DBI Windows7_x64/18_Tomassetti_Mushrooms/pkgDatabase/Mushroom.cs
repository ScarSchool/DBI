using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace pkgDatabase
{
    public enum MushroomUseful
    {
        EDIBLE,
        INEDIBLE,
        TOXIC
    }

    [Serializable]
    public class Mushroom : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private MushroomUseful useful;
        public MushroomUseful Useful
        {
            get { return useful; }
            set {
                useful = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Useful)));
            }
        }

        private string capColor;
        public string CapColor
        {
            get { return capColor; }
            set {
                capColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapColor)));
            }
        }

        private readonly string stemColor;
        public string StemColor
        {
            get { return stemColor; }
            set
            {
                capColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StemColor)));
            }
        }

        private readonly string description;
        public string Description
        {
            get { return description; }
            set
            {
                capColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        private readonly string imagePath;
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                capColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImagePath)));
            }
        }

        [XmlIgnore]
        public bool Updated;

        public event PropertyChangedEventHandler PropertyChanged;

        public Mushroom()
        {
            name = "undefined";
            capColor = "undefined";
            stemColor = "undefined";
            description = "description";
            imagePath = "file path";

            Updated = false;
        }

        public override string ToString()
        {
            return $"#{Id} {name} {Useful}";
        }
    }
}
