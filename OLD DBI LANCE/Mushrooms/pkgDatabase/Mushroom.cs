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
    public class Mushroom : NotifyPropertyChanged
    {
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set => setField(ref name, value, "Name");
        }

        private MushroomUseful useful;
        public MushroomUseful Useful
        {
            get => useful;
            set => setField(ref useful, value, "Useful");
        }

        private string capColor;
        public string CapColor
        {
            get => capColor;
            set => setField(ref capColor, value, "CapColor");
        }

        private string stemColor;
        public string StemColor
        {
            get => stemColor;
            set => setField(ref stemColor, value, "stemColor");
        }

        private string description;
        public string Description
        {
            get => description;
            set => setField(ref description, value, "Description");
        }

        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set => setField(ref imagePath, value, "ImagePath");
        }

        [XmlIgnore]
        public bool Modified;

        public Mushroom()
        {
            name = "undefined";
            capColor = "undefined";
            stemColor = "undefined";
            description = "description";
            imagePath = "file path";

            Modified = false;
        }

        public override string ToString()
        {
            return $"#{Id} {name} {Useful}";
        }
    }
}
