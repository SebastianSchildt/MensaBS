using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

/*
 * 
 * <tag value="Montag">
		<mittag>
			<essen nummer="1">
				<name>Milchreis mit Sauerkirschen, Zimt und Zucker </name>
				<remark>V</remark>
				<preis typ="stud">1,40</preis>
				<preis typ="bed">3,10</preis>
				<preis typ="gast">3,70</preis>
			</essen>
			<essen nummer="2">
				<name>HÃ¤hnchennuggets mit Chilidip </name>
				<remark>G</remark>
				<preis typ="stud">1,90</preis>
				<preis typ="bed">3,40</preis>
				<preis typ="gast">4,00</preis>
			</essen>
			<essen nummer="3">
				<name>RinderhÃ¼ftsteak mit KrÃ¤uterbutter und Wedges </name>
				<remark>R</remark>
				<preis typ="stud">3,60</preis>
				<preis typ="bed">5,30</preis>
				<preis typ="gast">5,90</preis>
			</essen>
			<essen nummer="4">
				<name>GemÃ¼se-Couscous-Pfanne </name>
				<remark>V</remark>
				<preis typ="stud">1,90</preis>
				<preis typ="bed">3,60</preis>
				<preis typ="gast">4,20</preis>
			</essen>
		</mittag>
		<abend>
			<essen nummer="5">
				<name>Rote Linsensuppe</name>
				<remark></remark>
				<preis typ="stud">1,40</preis>
				<preis typ="bed">3,10</preis>
				<preis typ="gast">3,70</preis>
			</essen>
			<essen nummer="6">
				<name>KartoffelkÃ¼chlein mit Apfelmus </name>
				<remark>3,V</remark>
				<preis typ="stud">1,80</preis>
				<preis typ="bed">3,50</preis>
				<preis typ="gast">4,10</preis>
			</essen>
			<essen nummer="7">
				<name>Gyros mit Zwiebeln AT </name>
				<remark>S</remark>
				<preis typ="stud">1,90</preis>
				<preis typ="bed">3,40</preis>
				<preis typ="gast">4,00</preis>
			</essen>
			<essen nummer="8">
				<name>1/2 BrathÃ¤hnchen </name>
				<remark>G</remark>
				<preis typ="stud">1,90</preis>
				<preis typ="bed">3,40</preis>
				<preis typ="gast">4,00</preis>
			</essen>
		</abend>
	</tag>
 * 
 * */




namespace MensaBS
{
    [XmlRoot("data")]
    public class TrivalgFeed
    {

        [XmlElement("tag")]
        public day tag;

        public class day
        {
            [XmlElement("mittag")]
            public foodlist lunch { get; set; }

            [XmlElement("abend")]
            public foodlist dinner { get; set; }


            public class foodlist
            {
                [XmlElement("essen")]
                public ObservableCollection<Meal> meals { get; set; }

            }
        }
      

    }
}
