using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics.ReportLayout
{
    public class ReportLayoutProvider : IReportLayoutProvider
    {
        public int CompanyId
        {
            set;
            get;
        }

        private string TableId
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        private DateTime LastDayOfTheMonth
        {
            get { return new DateTime(EndDate.Year, EndDate.Month, 1).AddMonths(1).AddDays(-1); }
        }

        private DateTime FirstDayOfTheMonth
        {
            get { return new DateTime(EndDate.Year, EndDate.Month, 1); }
        }

        private string BaseUrl
        {
            get { return "https://www.google.com/analytics/feeds/data"; }
        }

        public IReportLayout GetReportLayout()
        {
            if (CompanyId == 1) // bravura
            {
                TableId = "ga:21996217";
                ReportLayout bravuraLayout = GetLayoutWithVisitorsAdsAndFacebook();
                bravuraLayout.LogoImage = "bravura.png";

                var applications = GetGoalGraphUniqueEvents("ga:eventAction=~Application$", "Jobbansökningar");
                bravuraLayout.GoalGraphs.Add(applications);
                var registrations = GetGoalGraphUniqueEvents("ga:eventAction=~^Registration$", "Nya kandidater");
                bravuraLayout.GoalGraphs.Add(registrations);
                return bravuraLayout;
            }
            else if(CompanyId == 2) // sven eklund bil
            {
                TableId = "ga:53026108";
                ReportLayout eklundBilLayout = GetLayoutWithVisitorsAdsAndFacebook();
                eklundBilLayout.LogoImage = "eklund.gif";

                var visitorsTransport = GetGoalGraphUniqueVisitorsForUrl("\\?cat\\=4|transportbilar", "Besökare transportbilar");
                eklundBilLayout.GoalGraphs.Add(visitorsTransport);

                var actionsTransport = GetGoalGraphUniqueEvents("ga:eventCategory==transportbil", "Interaktioner transportbilar");
                actionsTransport.PieGraphQuery.Dimensions = "ga:eventAction";
                eklundBilLayout.GoalGraphs.Add(actionsTransport);

                eklundBilLayout.KeywordPositions = new List<string>()
                {
                    "Begagnad transportbil",
                    "Begagnade transportbilar Stockholm",
                    "Begangade transportbilar",
                    "Bilar bromma",
                    "Isuzu d-max",
                    "Transportbilar Bromma",
                    "Transportbilar Stockholm",
                };

                return eklundBilLayout;
            }
            else if (CompanyId == 3) // GEM General Equal Managment HB
            {
                TableId = "ga:53219144";
                ReportLayout gemLayout = GetLayoutWithVisitorsAdsAndFacebook();
                gemLayout.LogoImage = "gem.png";

                gemLayout.KeywordPositions = new List<string>()
                    {
                        "badrumsrenoveringar",
                        "designbadrum",
                        "designkök",
                        "dornbracht",
                        "exklusiva badrum",
                        "exklusiva kök",
                        "gaggenau",
                        "kvalitetskök",
                        "köksarkitekt",
                        "köksinredning",
                    };

                gemLayout.GoalGraphs.Add(GetGoalGraphUniqueVisitorsForUrl("bad|kok", "Besökare Bad eller Kök"));
                gemLayout.GoalGraphs.Add(GetVisitorDailyOneMonthGraph());
                
                return gemLayout;
            }
            else if (CompanyId == 4) // Linette Här i Sverige AB  
            {
                TableId = "ga:52706557";
                ReportLayout linetteLayout = GetLayoutWithVisitorsAdsAndFacebook();
                linetteLayout.LogoImage = "linette.png";

                var uniqueVisitorsWebbShop = GetGoalGraphUniquePageViewsForUrl("^/ehandel/ehandel.php$", "Besökare webbshopen");
                var uniqueVisitorsContact = GetGoalGraphUniquePageViewsForUrl("^/kontakt/$", "Besökare kontaktsida");

                linetteLayout.GoalGraphs.Add(uniqueVisitorsWebbShop);
                linetteLayout.GoalGraphs.Add(uniqueVisitorsContact);

                linetteLayout.KeywordPositions = new List<string>()
                {
                    "frisörgrossist",
                    "frisörinredning",
                    "frisörprodukter",
                    "frisörstolar",
                    "frisörverktyg",
                    "hårgrossist",
                    "salongsinredning"
                };

                return linetteLayout;
            }
            else if (CompanyId == 5) // Höglund
            {
                TableId = "ga:52835266";
                ReportLayout hoglund = GetLayoutWithVisitorsAdsAndFacebook();
                hoglund.LogoImage = "hoglunds.png";
                hoglund.GoalGraphs.Clear();


                var visitors = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare");
                var visitorsCarShop = GetGoalGraphUniqueVisitorsForUrl("/verkstad", "Besökare Verkstad");
                var visitorsCarWeeklyDeal = GetGoalGraphUniquePageViewsForUrl("fynd", "Besökare Veckans fynd");
                
                //var carAdviews = GetGoalGraphUniqueEvents("ga:eventAction=~adView", "Annonsvisningar Bilar i lager");
                
                //var visitorsNewCars = GetGoalGraphUniqueVisitorsForUrl("/mazda|/mercedes-benz|/chrysler", "Besökare, Mazda, Chrylser eller Mercedes-Benz");
                //var visitorsNissan = GetGoalGraphUniqueEvents("ga:eventLabel=~Nissan", "Besökare Nissan");

                var visitorsLandingPages = GetGoalGraphUniqueVisitorsForFilter("ga:pagePath==/mazdacx5/,ga:pagePath==/mazda2/", "Besökare kampanjsidor");

                //hoglund.GoalGraphs.Add(carAdviews);
                hoglund.GoalGraphs.Add(visitors);
                hoglund.GoalGraphs.Add(visitorsLandingPages);
                hoglund.GoalGraphs.Add(visitorsCarShop);
                hoglund.GoalGraphs.Add(visitorsCarWeeklyDeal);

                return hoglund;
            }
            else if (CompanyId == 6) // 6 Levande kök AB
            {
                TableId = "ga:53026901";
                ReportLayout ballingslovJarfalla = GetLayoutWithVisitorsAdsAndFacebook();
                ballingslovJarfalla.LogoImage = "levandekok.png";

                var visitorsWwwLrtv = GetGoalGraphUniqueVisitorsForFilter("", "Besökare www.levandekok.se");
                var pageViews = GetGoalGraphPageViewsWithFilter("", "Sidvisningar www.levandekok.se");
                var contactFormCompletions = GetGoalGraphUniqueEvents("ga:eventAction==Submited contact form", "Kontaktförfrågningar");

                ballingslovJarfalla.GoalGraphs.Add(visitorsWwwLrtv);
                ballingslovJarfalla.GoalGraphs.Add(pageViews); 
                ballingslovJarfalla.GoalGraphs.Add(contactFormCompletions);

                return ballingslovJarfalla;
            }
            else if (CompanyId == 7) // 7, "A. Lindqvist Radio & TV Aktiebolag"
            {
                TableId = "ga:53022756";
                ReportLayout lrtv = GetLayoutWithVisitorsAdsAndFacebook();
                lrtv.LogoImage = "lrtv.gif";

                var engagedVisitors = GetGoalGraphUniqueVisitorsForFilter("ga:visitLength=~^\\S\\S\\S\\S$|^\\S\\S\\S$","Besökare som stannar länge (>90 sekunder)");

                var visitorsWwwLrtv = GetGoalGraphUniqueVisitorsForFilter("ga:isMobile=~no|No|NO", "Besökare www.lrtv.se");
                lrtv.GoalGraphs.Add(visitorsWwwLrtv);
                lrtv.GoalGraphs.Add(engagedVisitors);

                return lrtv;
            }
            else if (CompanyId == 8) // 8, "Smart Recycling Sverige AB
            {
                TableId = "ga:53026505";
                ReportLayout smartRecyclingLayout = GetLayoutWithVisitorsAdsAndFacebook();
                smartRecyclingLayout.LogoImage = "smartrecycling.gif";

                var visitorsWebbPage = GetGoalGraphUniqueVisitorsForFilter("ga:isMobile=~no|No|NO", "Besökare www.smartrecycling.se");
                smartRecyclingLayout.GoalGraphs.Add(visitorsWebbPage);

                var visitorsContactLandingpage = GetGoalGraphUniqueEvents("ga:eventAction==Submited campaign offer", "Kontaktförfrågningar");
                smartRecyclingLayout.GoalGraphs.Add(visitorsContactLandingpage);

                return smartRecyclingLayout;
            }
            else if (CompanyId == 9) //  9, "Gyhne & Kristofferson Optik AB"
            {
                TableId = "ga:55051191";
                ReportLayout gkoptikRecyclingLayout = GetLayoutWithVisitorsAdsAndFacebook();
                gkoptikRecyclingLayout.LogoImage = "gkoptik.gif";

                var visitorsKontakt = GetGoalGraphUniqueVisitorsForUrl("contact", "Unika Besökare \"Kontakta Oss\"");
                var visitorsWebbPage = GetGoalGraphUniqueVisitorsForFilter("ga:isMobile=~no|No|NO", "Besökare www.gkoptik.se");
                
                gkoptikRecyclingLayout.GoalGraphs.Add(visitorsWebbPage);
                gkoptikRecyclingLayout.GoalGraphs.Add(visitorsKontakt);

                return gkoptikRecyclingLayout;
            }
            else if (CompanyId == 10) // Matteakuten Studybuddy AB
            {
                TableId = "ga:21154331"; //studybuddy

                ReportLayout masbReportLayout = new ReportLayout();
                masbReportLayout.LogoImage = "sbma.png";

                // graph 1
                var goalGraph1 = GetVisitorThreeMonthsGraph();
                goalGraph1.Title = "Studybuddy besökare";

                // graph 2
                var goalGraph2 = GetVisitorThreeMonthsGraph();
                goalGraph2.LineGraphQuery.Ids = "ga:7803723";
                goalGraph2.PieGraphQuery.Ids = "ga:7803723";
                goalGraph2.Title = "Matteakuten besökare";

                // graph 3
                string filterUrl = "^/registrera/$";
                var goalGraph3 = GetGoalGraphUniquePageViewsForUrl(filterUrl);
                goalGraph3.Title = "Studybuddy anmälningar";

                // graph 4
                string eventFilter = "ga:eventAction==anmälan";
                var goalGraph4 = GetGoalGraphTotalEvents(eventFilter);
                goalGraph4.Title = "Matteakuten anmälningar";
                goalGraph4.LineGraphQuery.Ids = "ga:7803723";
                goalGraph4.PieGraphQuery.Ids = "ga:7803723";

                masbReportLayout.GoalGraphs = new List<IGoalGraph>()
                                                     {
                                                         goalGraph1,
                                                         goalGraph2,
                                                         goalGraph3,
                                                         goalGraph4
                                                     };

                var adwordsQuery = GetAdPerformance();
                masbReportLayout.AdwordsQuery = new List<DataQuery>()
                                                       {
                                                           adwordsQuery
                                                       };
                

                IFacebookProfile facebookProfileOne = new FacebookProfile();
                facebookProfileOne.Title = "Facebookprofil Studybuddy";
                IFacebookProfile facebookProfileTwo = new FacebookProfile();
                facebookProfileTwo.Title = "Facebookprofil Matteakuten";

                masbReportLayout.FacebookProfiles = new List<IFacebookProfile>() { facebookProfileOne, facebookProfileTwo};

                return masbReportLayout;
            }
            else if (CompanyId == 11) //  11 scampi
            {
                TableId = "ga:39838410";
                ReportLayout scampi = GetLayoutWithVisitorsAdsAndFacebook();
                scampi.LogoImage = "scampi.png";

                var visitorsWebShop = GetGoalGraphUniqueVisitorsForUrl("/en/artiklar/new-collection/", "Unika Besökare Webbshop");
                var visitorsOutlet = GetGoalGraphUniqueVisitorsForUrl("/en/artiklar/new-collection/", "Unika Besökare Webbshop");
                var transactionsWebShop = GetGoalGraphTotalTransactions("Antal Transaktioner Webbshop");
                
                scampi.GoalGraphs.Add(visitorsWebShop);
                scampi.GoalGraphs.Add(visitorsOutlet);
                scampi.GoalGraphs.Add(transactionsWebShop);

                return scampi;
            }
            else if (CompanyId == 12) //  12, "Havoline Kemi AB"
            {
                TableId = "ga:53697719";
                ReportLayout havolineLayout = GetLayoutWithVisitorsAdsAndFacebook();
                havolineLayout.LogoImage = "hemtvätt.png";

                var visitorsWebShop = GetGoalGraphUniqueVisitorsForUrl("/store/", "Unika Besökare Webbshop");
                havolineLayout.GoalGraphs.Add(visitorsWebShop);

                var transactionsWebShop = GetGoalGraphTotalTransactions("Antal Transaktioner Webbshop");
                havolineLayout.GoalGraphs.Add(transactionsWebShop);

                return havolineLayout;
            }
            else if (CompanyId == 13) //  13, "Christian Hallberg Scandinavia AB"
            {
                TableId = "ga:55558448";
                ReportLayout christianHallbergLayout = GetLayoutWithVisitorsAdsAndFacebook();
                christianHallbergLayout.LogoImage = "christianHallbergScandinavia.jpg";

                return christianHallbergLayout;
            }
            else if (CompanyId == 14) // 14, "Hårgänget Södra AB");
            {
                TableId = "ga:55400943";
                ReportLayout harganetLayout = GetLayoutWithVisitorsAdsAndFacebook();
                harganetLayout.LogoImage = "harganget.png";

                var visitors = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare Harganget.com");


                var bookings = GetGoalGraphUniqueVisitorsForUrl("/boka-tid/boka-tid-online/", "Besökare boka tid online");

                var graph3 = GetGoalGraphUniqueVisitorsForUrl("/har-produkter/", "Besökare Hårprodukter");
               
                harganetLayout.GoalGraphs.Add(visitors);
                harganetLayout.GoalGraphs.Add(bookings);
                harganetLayout.GoalGraphs.Add(graph3);
                
                return harganetLayout;
            }
            else if (CompanyId == 15) // skidbytarboden
            {
                TableId = "ga:6032148";
                ReportLayout skidbytarbodenLayout = GetLayoutWithVisitorsAdsAndFacebook();
                skidbytarbodenLayout.LogoImage = "skidbytarboden.png";

                var visitorsShop = GetGoalGraphUniquePageViewsForUrl("/om-skidbytarboden/", "Besökare \"Hitta till butiken\"");
                skidbytarbodenLayout.GoalGraphs.Add(visitorsShop);
                
                var submitedContactForm = GetGoalGraphUniqueEvents("ga:eventAction=~Submited contact form", "Kontaktförfrågningar");
                skidbytarbodenLayout.GoalGraphs.Add(submitedContactForm);
                return skidbytarbodenLayout;
            }
            else if(CompanyId == 16) // Djursholms Golf Krog AB
            {
                TableId = "ga:54052347";
                ReportLayout djursholmsGolfKrogLayout = GetLayoutWithVisitorsAdsAndFacebook();
                djursholmsGolfKrogLayout.LogoImage = "djursholm.png";

                var visitorCatering = GetGoalGraphUniqueVisitorsForUrl("/catering", "Unika Besökare Catering");
                djursholmsGolfKrogLayout.GoalGraphs.Add(visitorCatering);
                var menuforTheWeek = GetGoalGraphUniqueVisitorsForUrl("/veckans", "Unika Besökare Lunch Meny");
                djursholmsGolfKrogLayout.GoalGraphs.Add(menuforTheWeek);
                	

                return djursholmsGolfKrogLayout;
            }
            else if(CompanyId == 17) // Tandläkare Melki AB
            {
                TableId = "ga:54049838";
                ReportLayout melkiLayout = GetLayoutWithVisitorsAdsAndFacebook();
                melkiLayout.LogoImage = "melki.jpg";

                var visitorsTretments = GetGoalGraphUniqueVisitorsForUrl("/behandlingar.html", "Unika Besökare \"Behandlingar\"");
                melkiLayout.GoalGraphs.Add(visitorsTretments);
                var visitorsContact = GetGoalGraphUniqueVisitorsForUrl("/kontakta-oss.html", "Unika Besökare \"Kontakta Oss\"");
                melkiLayout.GoalGraphs.Add(visitorsContact);

                melkiLayout.FacebookProfiles.Clear();
                return melkiLayout;
            }
            else if (CompanyId == 18) // Ryska posten
            {
                TableId = "ga:42470709";
                ReportLayout ryskaposten = GetLayoutWithVisitorsAdsAndFacebook();
                ryskaposten.LogoImage = "ryskaposten.png";

                var visitorsWeb = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare ryskaposten.se");

                var visitorsContact = GetGoalGraphUniqueVisitorsForUrl("/tjänster/event/", "Besökare Event");
                ryskaposten.GoalGraphs.Add(visitorsWeb);
                ryskaposten.GoalGraphs.Add(visitorsContact);

                ryskaposten.FacebookProfiles.Clear();
                return ryskaposten;
            }
            else if (CompanyId == 19) // iStone Xplore
            {
                TableId = "ga:55629302";
                ReportLayout iStoneXplore = GetLayoutWithVisitorsAdsAndFacebook();
                iStoneXplore.LogoImage = "istone_xplore.png";

                var visitorsWeb = GetGoalGraphUniqueVisitorsForUrl("/web/Hem.aspx", "Unika Besökare xplore.istone.se");

                var visitorsContact = GetGoalGraphUniqueVisitorsForUrl("/web/Kontakt.aspx", "Unika Besökare Kontakta oss");
                iStoneXplore.GoalGraphs.Add(visitorsWeb);
                iStoneXplore.GoalGraphs.Add(visitorsContact);

                iStoneXplore.FacebookProfiles.Clear();
                return iStoneXplore;
            }
            else if (CompanyId == 20) // Söderkök & luckor AB
            {
                TableId = "ga:56066061";
                ReportLayout soderkokLayout = GetLayoutWithVistitorsAndAds();
                soderkokLayout.LogoImage = "soderkok.jpg";

                var visitorsWww = GetGoalGraphUniqueVisitorsForFilter("", "Besökare www.soderkok-luckor.se");
                var contacts = GetGoalGraphUniqueEvents("ga:eventCategory=~Contact", "Kontaktförfrågningar");
                var visitorsFronts = GetGoalGraphUniqueVisitorsForUrl("/luckor/", "Besökare Luckor");
                var visitorsInspiration = GetGoalGraphUniqueVisitorsForUrl("/inspiration/", "Besökare Inspiration");

                soderkokLayout.GoalGraphs.Add(visitorsWww);
                soderkokLayout.GoalGraphs.Add(contacts);
                soderkokLayout.GoalGraphs.Add(visitorsFronts);
                soderkokLayout.GoalGraphs.Add(visitorsInspiration);
                
                soderkokLayout.FacebookProfiles.Clear();
                return soderkokLayout;
            }
            else if (CompanyId == 21) // Viessmann Värmeteknik AB
            {
                TableId = "ga:56271815";
                ReportLayout viessmannLayout = GetLayoutWithVistitorsAndAds();
                viessmannLayout.LogoImage = "viessmann.jpg";

                var infomrationDownloads = GetGoalGraphUniqueEvents("ga:eventCategory=~Downloads", "Nedladdningar av manualer");
                var readMore = GetGoalGraphUniqueEvents("ga:eventAction=~www.viessmann.se", "Läs mer om produkten på viessmann.se");
                var lookProdukt = GetGoalGraphUniqueEvents("ga:eventAction=~Clicked on product", "Visningar av modeller");
                
                viessmannLayout.GoalGraphs.Add(infomrationDownloads);
                viessmannLayout.GoalGraphs.Add(readMore);
                viessmannLayout.GoalGraphs.Add(lookProdukt);

                return viessmannLayout;
            }
            else if (CompanyId == 22) // Helloy Nordic AB
            {
                TableId = "ga:46680574";
                ReportLayout helloyLayout = GetLayoutWithVisitorsAdsAndFacebook();
                helloyLayout.LogoImage = "helloy.png";

                var visitorsWebpage = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare hemsidan");
                //var infomrationDownloads = GetGoalGraphUniqueEvents("ga:eventCategory=~contact", "Kontaktförfrågningar");
                var visitorsCases = GetGoalGraphUniqueVisitorsForUrl("kundcase", "Besökare hemsidan");

                helloyLayout.GoalGraphs.Add(visitorsWebpage);
                //helloyLayout.GoalGraphs.Add(infomrationDownloads);
                helloyLayout.GoalGraphs.Add(visitorsCases);

                return helloyLayout;
            }
            else if (CompanyId == 23) // Villa Källhagen Hotell och Restaurang AB
            {
                TableId = "ga:63535236";
                ReportLayout kallhagenLayout = GetLayoutWithVisitorsAdsAndFacebook();
                kallhagenLayout.LogoImage = "kallhagen.jpg";

                var visitorsResturant = GetGoalGraphUniqueVisitorsForUrl("restaurang", "Besökare restaurang");
                var visitorsHotel = GetGoalGraphUniqueVisitorsForUrl("hotell", "Besökare hotell");

                kallhagenLayout.GoalGraphs.Add(visitorsResturant);
                kallhagenLayout.GoalGraphs.Add(visitorsHotel);

                return kallhagenLayout;
            }
            else if (CompanyId == 24) // 24 Pewe Billackering AB
            {
                TableId = "ga:58162706";
                ReportLayout peweLayout = GetLayoutWithVistitorsAndAds();
                peweLayout.LogoImage = "pewe.jpg";

                var visitorsWww = GetGoalGraphUniqueVisitorsForFilter("", "Besökare Pewebillackering.se");
                var visitorsTimeBooking = GetGoalGraphUniqueVisitorsForUrl("/boka-tid/", "Besökare boka tid");
                var visitorsDropIn = GetGoalGraphUniqueVisitorsForUrl("/drop-in/", "Besökare Drop-in");
                var visitorsPainting = GetGoalGraphUniqueVisitorsForUrl("/billackering/", "Besökare Lackering");

                peweLayout.GoalGraphs.Add(visitorsWww);
                peweLayout.GoalGraphs.Add(visitorsTimeBooking);
                peweLayout.GoalGraphs.Add(visitorsDropIn);
                peweLayout.GoalGraphs.Add(visitorsPainting);

                return peweLayout;
            }
            else if (CompanyId == 25) // Kök & inredning Stockholm AB
            {
                TableId = "ga:59431754";
                ReportLayout claessonKok = GetLayoutWithVistitorsAndAds();
                claessonKok.LogoImage = "claessonkok.png";

                var visitorToClaesonkokse = GetGoalGraphUniqueEvents("ga:eventLabel=~claessonkok.se/$|claessonkok.se$", "Besökare till www.claessonkok.se");
                var downloads = GetGoalGraphUniqueEvents("ga:eventLabel=~objid", "Katalog Öppningar");

                claessonKok.GoalGraphs.Add(visitorToClaesonkokse);
                claessonKok.GoalGraphs.Add(downloads);

                return claessonKok;
            }
            else if (CompanyId == 26) // Folkhemmet Möbler & Inredning AB
            {
                TableId = "ga:60695483";
                ReportLayout folkhemmetLayout = GetLayoutWithVisitorsAdsAndFacebook();
                folkhemmetLayout.LogoImage = "folkhemmet.jpg";

                var visitorsWebpage = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare hemsidan");
                var visitorsWebshop = GetGoalGraphUniqueVisitorsForUrl("/webshop", "Besökare webbshop");
                
                folkhemmetLayout.GoalGraphs.Add(visitorsWebpage);
                folkhemmetLayout.GoalGraphs.Add(visitorsWebshop);

                return folkhemmetLayout;
            }
            else if (CompanyId == 27) // Fast lane 
            {
                TableId = "ga:15370107";
                ReportLayout fastLane = GetLayoutWithVisitorsAdsAndFacebook();
                fastLane.LogoImage = "fastlane.jpg";

                var visitorsWebb = GetGoalGraphUniqueVisitorsForUrl("/", "Unika besökare www.flane.se");
                var visitorsBooking = GetGoalGraphUniqueVisitorsForUrl("/booking", "Unika besökare /booking");

                fastLane.GoalGraphs.Add(visitorsWebb);
                fastLane.GoalGraphs.Add(visitorsBooking);

                return fastLane;
            }
            else if (CompanyId == 28) // earn more ab
            {
                TableId = "ga:58724113";
                ReportLayout kg10Layout = GetLayoutWithVisitorsAdsAndFacebook();
                kg10Layout.LogoImage = "kg10.jpg";

                var contactVisits = GetGoalGraphUniqueVisitorsForUrl("/kontakta-kontorshotell/",
                                                                     "Unika besökare kontakt");
                var logins = GetGoalGraphUniqueVisitorsForUrl("/login/", "Antal inloggningar");

                kg10Layout.GoalGraphs.Add(contactVisits);
                kg10Layout.GoalGraphs.Add(logins); 

                return kg10Layout;
            }
            else if (CompanyId == 29) // Internet border technologies
            {
                TableId = "ga:3911371";
                ReportLayout internetBorderLayout = GetLayoutWithVistitorsAndAds();
                internetBorderLayout.LogoImage = "internet-border.png";

                var visitors = GetGoalGraphUniqueVisitorsForUrl("/webbpublicering.html", "Besökare Webbpublicering");
                var itdrift = GetGoalGraphUniqueVisitorsForUrl("/itdrift.html", "Besökare IT-drift");

                internetBorderLayout.GoalGraphs.Add(visitors);
                internetBorderLayout.GoalGraphs.Add(itdrift);
                

                return internetBorderLayout;
            }
            else if (CompanyId == 30) // Familjebil-center OE AB
            {
                TableId = "ga:58730902";
                ReportLayout famBilLayout = GetLayoutWithVisitorsAdsAndFacebook();
                famBilLayout.LogoImage = "fambil.png";

                var graph1 = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare www.fambil.se");
                var graph2 = GetGoalGraphUniqueVisitorsForUrl("/billager.htm", "Besökare Bilar i lager");

                famBilLayout.GoalGraphs.Add(graph1);
                famBilLayout.GoalGraphs.Add(graph2);

                return famBilLayout;
            }
            else if (CompanyId == 31) // Sten lindgrens
            {
                TableId = "ga:59407792";
                ReportLayout stenLindsgrensLayout = GetLayoutWithVisitorsAdsAndFacebook();
                stenLindsgrensLayout.LogoImage = "sten-lindgren.png";

                var graph1 = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare");
                var graph2 = GetGoalGraphUniqueVisitorsForUrl("/produkter/", "Besökare Produkter");

                stenLindsgrensLayout.GoalGraphs.Add(graph1);
                stenLindsgrensLayout.GoalGraphs.Add(graph2);

                return stenLindsgrensLayout;
            }
            else if (CompanyId == 32) // Adeo care
            {
                TableId = "ga:60969580";
                ReportLayout adeoLayout = GetLayoutWithVistitorsAndAds();
                adeoLayout.LogoImage = "adeocare.jpg";    

                var visitors = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare hemsidan");
                var interactions = GetGoalGraphUniqueEvents("ga:eventCategory=~Landningssida", "Interaktioner landningssida");

                adeoLayout.GoalGraphs.Add(visitors);
                adeoLayout.GoalGraphs.Add(interactions);

                return adeoLayout;
            }
            else if (CompanyId == 34) // Colibri Resort
            {
                TableId = "ga:64222527";
                ReportLayout colibriLayout = GetLayoutWithVisitorsAdsAndFacebook();
                colibriLayout.LogoImage = "colibriresort.jpg";

                var visitorsWWW = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare ilhagrandehotel.com");
                var contactFormCompletions = GetGoalGraphUniqueEvents("ga:eventAction==Submited contact form", "Kontaktförfrågningar");

                colibriLayout.GoalGraphs.Add(visitorsWWW);
                colibriLayout.GoalGraphs.Add(contactFormCompletions);
                return colibriLayout;
            }
            else if (CompanyId == 37) //Earbooks AB
            {
                TableId = "ga:36741714";
                ReportLayout aktivaungdommarLayout = GetLayoutWithVistitorsAndAds();
                aktivaungdommarLayout.LogoImage = "aktiva-ungdommar.jpg";    

                var visitorsStart = GetGoalGraphUniqueVisitorsForUrl("/bestall-material/starta-en-forsaljning", "Besökare starta en försäljning");
                var visitorsInfo = GetGoalGraphUniqueVisitorsForUrl("/bestall-material/bestall-kostnadsfri-information", "Besökare beställ kostnadsfri information");
                
                aktivaungdommarLayout.GoalGraphs.Add(visitorsStart);
                aktivaungdommarLayout.GoalGraphs.Add(visitorsInfo);

                return aktivaungdommarLayout;
            }
            else if (CompanyId == 38) // Enoterra AB
            {
                TableId = "ga:36868139";
                ReportLayout enoterraLayout = GetLayoutWithVisitorsAdsAndFacebook();
                enoterraLayout.LogoImage = "enoterra.jpg";

                var visitorsWWW = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare enoterra.se");
                var visitorsNews = GetGoalGraphUniqueVisitorsForUrl("nyheter|2012", "Besökare Nyheter");
                var visitorsProducts = GetGoalGraphUniqueVisitorsForUrl("/produkter/", "Besökare Produkter");

                enoterraLayout.GoalGraphs.Add(visitorsWWW);
                enoterraLayout.GoalGraphs.Add(visitorsNews);
                enoterraLayout.GoalGraphs.Add(visitorsProducts);

                return enoterraLayout;
            }
            else if (CompanyId == 46) // Welin & Co AB
            {
                TableId = "ga:46262867";
                ReportLayout welinLayout = GetLayoutWithVistitorsAndAds();
                welinLayout.LogoImage = "welin.jpg";

                var visitorsWWW = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare www.welinoco.com/");
                var visitorsKlick = GetGoalGraphUniqueVisitorsForUrl("/welin-klickstav--c-521-1.aspx", "Besökare Klickstav");

                welinLayout.GoalGraphs.Add(visitorsWWW);
                welinLayout.GoalGraphs.Add(visitorsKlick);
                return welinLayout;
            }
            else if (CompanyId == 52) // Ballingslov Infracity
            {
                TableId = "ga:61748778";
                ReportLayout infracityLayout = GetLayoutWithVisitorsAdsAndFacebook();
                infracityLayout.LogoImage = "ballingslovinfracity.jpg";

                var visitorsWWW = GetGoalGraphUniqueVisitorsForUrl("/", "Besökare ballingslov-infracity.se");
                var visitorsAbout = GetGoalGraphUniqueVisitorsForUrl("/hitta-till-ballingslov-infracity/", "Besökare Om oss");
                var visitorsKitchen = GetGoalGraphUniqueVisitorsForUrl("/ballingslov-kok/", "Besökare Kök");

                infracityLayout.GoalGraphs.Add(visitorsWWW);
                infracityLayout.GoalGraphs.Add(visitorsAbout);
                infracityLayout.GoalGraphs.Add(visitorsKitchen);
                return infracityLayout;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private DataQuery GetAdPerformance()
        {
            DataQuery adwordsQuery = new DataQuery(BaseUrl);
            SetOneMonthQuery(adwordsQuery);
            adwordsQuery.Dimensions = "ga:campaign";
            adwordsQuery.Metrics = "ga:adCost,ga:CPC,ga:adClicks";
            adwordsQuery.Filters = "ga:adCost!=0";
            return adwordsQuery;
        }


        private GoalGraph GetGoalGraphTotalEvents(string eventFilter)
        {
            GoalGraph goalGraph4 = new GoalGraph();

            DataQuery registrationsMatteakuten = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsMatteakuten);

            registrationsMatteakuten.Metrics = "ga:totalEvents";
            registrationsMatteakuten.Dimensions = "ga:month,ga:year";
            registrationsMatteakuten.Sort = "ga:year,ga:month";

            registrationsMatteakuten.Filters = eventFilter;
            goalGraph4.LineGraphQuery = registrationsMatteakuten;

            var registrationsMatteakutenSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsMatteakutenSources);
            registrationsMatteakutenSources.Metrics = "ga:totalEvents";
            registrationsMatteakutenSources.Dimensions = "ga:source,ga:medium";
            registrationsMatteakutenSources.Sort = "-ga:totalEvents";
            registrationsMatteakutenSources.Filters = eventFilter;
            goalGraph4.PieGraphQuery = registrationsMatteakutenSources;
            return goalGraph4;
        }

        private GoalGraph GetGoalGraphUniqueEvents(string eventFilter, string title="")
        {
            GoalGraph graph = new GoalGraph();

            DataQuery registrationsMatteakuten = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsMatteakuten);

            registrationsMatteakuten.Metrics = "ga:uniqueEvents";
            registrationsMatteakuten.Dimensions = "ga:month,ga:year";
            registrationsMatteakuten.Sort = "ga:year,ga:month";

            registrationsMatteakuten.Filters = eventFilter;
            graph.LineGraphQuery = registrationsMatteakuten;

            var registrationsMatteakutenSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsMatteakutenSources);
            registrationsMatteakutenSources.Metrics = "ga:uniqueEvents";
            registrationsMatteakutenSources.Dimensions = "ga:source,ga:medium";
            registrationsMatteakutenSources.Sort = "-ga:uniqueEvents";
            registrationsMatteakutenSources.Filters = eventFilter;
            registrationsMatteakutenSources.NumberToRetrieve = 6;
            graph.PieGraphQuery = registrationsMatteakutenSources;
            graph.AddOthers = true;
            graph.Title = title;
            
            return graph;
        }


        private GoalGraph GetGoalGraphUniquePageViewsForUrl(string filterUrl, string title = "")
        {
            GoalGraph goalGraph3 = new GoalGraph();
            DataQuery registrationsStudybuddy = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsStudybuddy);
            registrationsStudybuddy.Dimensions = "ga:month,ga:year";
            registrationsStudybuddy.Sort = "ga:year,ga:month";
            registrationsStudybuddy.Metrics = "ga:uniquePageviews";
            registrationsStudybuddy.Filters = "ga:pagePath=~" + filterUrl;
            registrationsStudybuddy.NumberToRetrieve = 3;
            goalGraph3.LineGraphQuery = registrationsStudybuddy;

            DataQuery registrationsSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsSources);
            registrationsSources.Dimensions = "ga:source,ga:medium";
            registrationsSources.Metrics = "ga:uniquePageviews";
            registrationsSources.Filters = "ga:pagePath=~" + filterUrl;
            registrationsSources.Sort = "-ga:uniquePageviews";
            registrationsSources.NumberToRetrieve = 5;
            goalGraph3.PieGraphQuery = registrationsSources;

            goalGraph3.Title = title;

            return goalGraph3;
        }

        private GoalGraph GetGoalGraphPageViewsWithFilter(string filter, string title = "")
        {
            GoalGraph goalGraph3 = new GoalGraph();
            DataQuery registrationsStudybuddy = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsStudybuddy);
            registrationsStudybuddy.Dimensions = "ga:month,ga:year";
            registrationsStudybuddy.Sort = "ga:year,ga:month";
            registrationsStudybuddy.Metrics = "ga:pageviews";
            registrationsStudybuddy.Filters = filter;
            registrationsStudybuddy.NumberToRetrieve = 3;
            goalGraph3.LineGraphQuery = registrationsStudybuddy;

            DataQuery registrationsSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsSources);
            registrationsSources.Dimensions = "ga:source,ga:medium";
            registrationsSources.Metrics = "ga:pageviews";
            registrationsSources.Filters = filter;
            registrationsSources.Sort = "-ga:pageviews";
            registrationsSources.NumberToRetrieve = 5;
            goalGraph3.PieGraphQuery = registrationsSources;

            goalGraph3.Title = title;

            return goalGraph3;
        }


        private GoalGraph GetGoalGraphUniqueVisitorsForUrl(string filterUrl, string title = "")
        {
            GoalGraph goalGraph3 = new GoalGraph();
            DataQuery registrationsStudybuddy = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsStudybuddy);
            registrationsStudybuddy.Dimensions = "ga:month,ga:year";
            registrationsStudybuddy.Sort = "ga:year,ga:month";
            registrationsStudybuddy.Metrics = "ga:visitors";
            registrationsStudybuddy.Filters = "ga:pagePath=~" + filterUrl;
            registrationsStudybuddy.NumberToRetrieve = 3;
            goalGraph3.LineGraphQuery = registrationsStudybuddy;

            DataQuery registrationsSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsSources);
            registrationsSources.Dimensions = "ga:source,ga:medium";
            registrationsSources.Metrics = "ga:visitors";
            registrationsSources.Filters = "ga:pagePath=~" + filterUrl;
            registrationsSources.Sort = "-ga:visitors";
            registrationsSources.NumberToRetrieve = 5;
            goalGraph3.PieGraphQuery = registrationsSources;

            goalGraph3.Title = title;

            return goalGraph3;
        }

        private GoalGraph GetGoalGraphUniqueVisitorsForFilter(string filter, string title = "")
        {
            GoalGraph goalGraph3 = new GoalGraph();
            DataQuery registrationsStudybuddy = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsStudybuddy);
            registrationsStudybuddy.Dimensions = "ga:month,ga:year";
            registrationsStudybuddy.Sort = "ga:year,ga:month";
            registrationsStudybuddy.Metrics = "ga:visitors";
            registrationsStudybuddy.Filters = filter;
            registrationsStudybuddy.NumberToRetrieve = 3;
            goalGraph3.LineGraphQuery = registrationsStudybuddy;

            DataQuery registrationsSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsSources);
            registrationsSources.Dimensions = "ga:source,ga:medium";
            registrationsSources.Metrics = "ga:visitors";
            registrationsSources.Filters = filter;
            registrationsSources.Sort = "-ga:visitors";
            registrationsSources.NumberToRetrieve = 5;
            goalGraph3.PieGraphQuery = registrationsSources;

            goalGraph3.Title = title;

            return goalGraph3;
        }

        private GoalGraph GetGoalGraphTotalTransactions( string title = "")
        {
            GoalGraph goalGraph3 = new GoalGraph();
            DataQuery registrationsStudybuddy = new DataQuery(BaseUrl);
            SetThreeMonthQuery(registrationsStudybuddy);
            registrationsStudybuddy.Dimensions = "ga:month,ga:year";
            registrationsStudybuddy.Sort = "ga:year,ga:month";
            registrationsStudybuddy.Metrics = "ga:transactions";
            registrationsStudybuddy.NumberToRetrieve = 3;
            goalGraph3.LineGraphQuery = registrationsStudybuddy;

            DataQuery registrationsSources = new DataQuery(BaseUrl);
            SetOneMonthQuery(registrationsSources);
            registrationsSources.Dimensions = "ga:source,ga:medium";
            registrationsSources.Metrics = "ga:transactions";
            registrationsSources.Sort = "-ga:transactions";
            registrationsSources.NumberToRetrieve = 5;
            goalGraph3.PieGraphQuery = registrationsSources;

            goalGraph3.Title = title;

            return goalGraph3;
        }

        private GoalGraph GetVisitorThreeMonthsGraph()
        {
            GoalGraph goalGraph1 = new GoalGraph();
            goalGraph1.Title = "Besökare";


            DataQuery visitorSiteQuery = new DataQuery(BaseUrl);
            SetThreeMonthQuery(visitorSiteQuery);
            visitorSiteQuery.Dimensions = "ga:month,ga:year";
            visitorSiteQuery.Sort = "ga:year,ga:month";
            visitorSiteQuery.Metrics = "ga:visitors";
            visitorSiteQuery.NumberToRetrieve = 3;
            goalGraph1.LineGraphQuery = visitorSiteQuery;

            DataQuery sourceSiteVisitorsQuery = new DataQuery(BaseUrl);
            SetOneMonthQuery(sourceSiteVisitorsQuery);
            sourceSiteVisitorsQuery.Dimensions = "ga:isMobile";
            sourceSiteVisitorsQuery.Metrics = "ga:visitors";
            sourceSiteVisitorsQuery.NumberToRetrieve = 2;
            goalGraph1.PieGraphQuery = sourceSiteVisitorsQuery;
            goalGraph1.AddOthers = false;

            goalGraph1.AddFacebookVisitorsMonthlyToLineGraph = true;
            goalGraph1.AddFacebookVisitorsToLineGraph = true;

            goalGraph1.AddFacebookVisitorsToPieGraph = true;
            goalGraph1.AddFacebookVisitorsMonthlyToPieGraph = false;

            return goalGraph1;
        }

        private GoalGraph GetVisitorDailyOneMonthGraph()
        {
            GoalGraph vistorsGraph = new GoalGraph();
            vistorsGraph.Title = "Besökare under månaden";

            DataQuery visitorSiteQuery = new DataQuery(BaseUrl);
            SetOneMonthQuery(visitorSiteQuery);
            visitorSiteQuery.Dimensions = "ga:day,ga:month";
            visitorSiteQuery.Sort = "ga:month,ga:day";
            visitorSiteQuery.Metrics = "ga:visitors";
            visitorSiteQuery.NumberToRetrieve = 31;
            vistorsGraph.LineGraphQuery = visitorSiteQuery;

            DataQuery sourceSiteVisitorsQuery = new DataQuery(BaseUrl);
            SetOneMonthQuery(sourceSiteVisitorsQuery);
            sourceSiteVisitorsQuery.Dimensions = "ga:source,ga:medium";
            sourceSiteVisitorsQuery.Metrics = "ga:visitors";
            sourceSiteVisitorsQuery.NumberToRetrieve = 6;
            sourceSiteVisitorsQuery.Sort = "-ga:visitors";
            vistorsGraph.PieGraphQuery = sourceSiteVisitorsQuery;
            vistorsGraph.AddOthers = true;
            vistorsGraph.SumTotal = true;

            return vistorsGraph;
        }

        private IList<DataQuery> GetKeywordPerfromances(IList<string> keywords)
        {
            string sourceFilter = "ga:source=~google";
            List<String> keywordFilters = new List<string>();
            
            foreach (var keyword in from keyword in keywords select "(^" + keyword + "$)")
            {
                bool isNewFilter = keywordFilters.Count==0 || String.IsNullOrEmpty(keywordFilters.Last());
                if(isNewFilter)
                {
                    keywordFilters.Add("ga:keyword=~"+keyword);
                    continue;
                }

                bool hasRoomForMoreKeywords = keyword.Length + keywordFilters.Last().Length < 128 - sourceFilter.Length - 5;
                if(hasRoomForMoreKeywords)
                {
                    keywordFilters[keywordFilters.Count - 1] = keywordFilters.Last() + "|" + keyword;
                }
                else
                {
                    keywordFilters.Add("ga:keyword=~"+keyword);
                }
            }

            var queries = new List<DataQuery>();
            foreach (var keywordFilter in keywordFilters)
            {
                DataQuery keywordQuery = new DataQuery(BaseUrl);
                SetOneMonthQuery(keywordQuery);
                keywordQuery.Dimensions = "ga:medium,ga:keyword";
                keywordQuery.Metrics = "ga:visitors";
                keywordQuery.Filters = keywordFilter + ";" + sourceFilter;

                queries.Add(keywordQuery);
            }
            return queries;
        }


        private void SetThreeMonthQuery(DataQuery query)
        {
            query.Ids = TableId;
            query.GAStartDate = FirstDayOfTheMonth.AddMonths(-2).ToString("yyyy-MM-dd");
            query.GAEndDate = LastDayOfTheMonth.ToString("yyyy-MM-dd");
        }

        private void SetOneMonthQuery(DataQuery query)
        {
            query.Ids = TableId;
            query.GAStartDate = FirstDayOfTheMonth.ToString("yyyy-MM-dd");
            query.GAEndDate = LastDayOfTheMonth.ToString("yyyy-MM-dd");
        }

        private ReportLayout GetLayoutWithVisitorsAdsAndFacebook()
        {
            var reportLayout = GetLayoutWithVistitorsAndAds();

            IFacebookProfile facebookProfileOne = new FacebookProfile();
            facebookProfileOne.Title = "Facebookprofil";

            reportLayout.FacebookProfiles = new List<IFacebookProfile>() { facebookProfileOne };

            return reportLayout;
        }

        private ReportLayout GetLayoutWithVistitorsAndAds()
        {
            ReportLayout reportLayout = new ReportLayout();

            // graph 1
            var goalGraph1 = GetVisitorThreeMonthsGraph();
            goalGraph1.Title = "Besökare";


            reportLayout.GoalGraphs = new List<IGoalGraph>()
                                          {
                                              goalGraph1,
                                          };

            var adwordsQuery = GetAdPerformance();
            reportLayout.AdwordsQuery = new List<DataQuery>()
                                            {
                                                adwordsQuery
                                            };
            return reportLayout;
        }
    }
}
