using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Xml;
using System.Threading;

namespace Genetik
{

    static class Program
    {
        private static readonly Random rand = new Random();

        /// <summary>
        /// Dikkat !!! Backpack algortmasına gidecek veri "BackpackeGidecekVeriler" içerisindedir!!!
        /// </summary>

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Değişken tanımlamaları
            Form1 form1 = new Form1();
            var chart = new Chart();
            var chartArea = new ChartArea();
            List<double[]> Kurlar = new List<double[]>();           // Kurların yazıldığı liste.
            Label birimBasiKazancLabel = new Label();
            Label enCokKazandıranlarLabel = new Label();
            Label bilgilerLabel = new Label();
            Label enCokKazandiranlarinKurlariLabel = new Label();
            List<int> kazandıranKurBilgileri = new List<int>();     // Kazandıran döviz bilgilerinin tutulduğu liste.
            List<double> BackpackeGidecekVeriler = new List<double>();
            List<string> kurAdlari = new List<string>();
            int uzaklik = 0;
            double anaPara = 1000;
            int kromozomUzunlugu = 3;
            int nufusBuyuklugu = 5;
            #endregion

            form1.BackColor = System.Drawing.Color.White;                       // Form konfigürasyou.
            form1.Size = new Size(1350, 850);

            xmlOku(Kurlar,kurAdlari);

            double[] egimler = new double[Kurlar.Count];                        // Hesaplanan eğimlerin tutulduğu dizi.

            #region Tablolar ve konfigürasyonlar
            form1.Controls.Add(chart);                                          // Forma tablo ekliyoruz.
            chart.Size = new Size(780, 600);

            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.LabelStyle.Font = new Font("Consolas", 8);
            chartArea.AxisY.LabelStyle.Font = new Font("Consolas", 8);
            chartArea.AxisY.Title = "Kur değeri (nokta notasyonlar için) \nY koordinatı (çizgi notasyonlar için) ";
            chartArea.AxisX.Title = "Zaman (nokta notasyonlar için)\nX koordiatı (çizgi notasyonlar için)";

            chart.ChartAreas.Add(chartArea);

            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 0.1;
            chart.ChartAreas[0].AxisX.Minimum = -25;                            // Tablo konfigürasyonları.
            chart.ChartAreas[0].AxisX.Maximum = 25;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 5;

            birimBasiKazancLabel.Location = new Point(chart.Size.Width - 5, 60);
            birimBasiKazancLabel.Font = new Font("Times New Roman", 12);
            birimBasiKazancLabel.Text = "1 birim başına kazançlar";
            birimBasiKazancLabel.AutoSize = true;
            form1.Controls.Add(birimBasiKazancLabel);

            bilgilerLabel.Text = "Bilgiler";
            bilgilerLabel.Font = new Font("Times New Roman", 15);
            bilgilerLabel.Location = new Point(chart.Size.Width, 20);
            #endregion

            #region En çok kazandıranların kurları label
            enCokKazandiranlarinKurlariLabel.Location = new Point(chart.Size.Width + 400, 60);
            enCokKazandiranlarinKurlariLabel.Font = new Font("Times New Roman", 12);
            enCokKazandiranlarinKurlariLabel.Text = "Son kurları";
            enCokKazandiranlarinKurlariLabel.AutoSize = true;
            form1.Controls.Add(enCokKazandiranlarinKurlariLabel);
            form1.Controls.Add(bilgilerLabel);
            #endregion

            #region En çok kazandıranlar label
            uzaklik++;
            enCokKazandıranlarLabel.Location = new Point(chart.Size.Width + 200, 60);
            enCokKazandıranlarLabel.Font = new Font("Times New Roman", 12);
            enCokKazandıranlarLabel.Text = "En çok kazandıranlar";
            enCokKazandıranlarLabel.AutoSize = true;
            form1.Controls.Add(enCokKazandıranlarLabel);
            #endregion

            dogruEgimi(Kurlar, egimler);

            seriesOlustur(egimler, chart, kurAdlari, Kurlar);

            int distance = birimBasinaKazandirma(egimler, form1, uzaklik, kurAdlari, chart, BackpackeGidecekVeriler);

            //Backpake gidecek veriler list içinde. Gönderirken toArray metdu ile gönderilebilir. BackpackeGidecekVeriler.ToArray() şeklinde.

            enCokKazandiranlariGoster(egimler, chart, form1, kurAdlari, kazandıranKurBilgileri);

            double[] kazandiranlarinSonKurlari = new double[kazandıranKurBilgileri.Count()];
            int[] kazananKurIndisleri = new int[kazandıranKurBilgileri.Count()];

            kazandiranlarinBilgileri(kazandıranKurBilgileri, kazandiranlarinSonKurlari, kazananKurIndisleri, Kurlar, chart, kurAdlari, form1);

            Label paramız = new Label();
            paramız.Location = new Point(chart.Size.Width, (53 + (distance * 30)));
            paramız.Text = "Para Miktarınız:";
            paramız.Width = 80;
            form1.Controls.Add(paramız);

            TextBox para = new TextBox();
            para.Location = new Point(chart.Size.Width + 80, (50 + (distance * 30)));
            form1.Controls.Add(para);

            Button hesaplaBtn = new Button();
            hesaplaBtn.Location = new Point(chart.Size.Width + 185, (48 + (distance*30)));
            hesaplaBtn.Text = "Hesapla";
            form1.Controls.Add(hesaplaBtn);
            hesaplaBtn.Click += (s, e) =>
            {
                var ilkNesil = populasyonUretme(Convert.ToInt32(para.Text), 6, 3);
                var sonuc = iterator(ilkNesil, kazandiranlarinSonKurlari);
                var ekranaGidecekler = optimum(sonuc, kazandiranlarinSonKurlari);

                Label döviz1 = new Label();
                döviz1.Location = new Point(chart.Size.Width , (80 + (distance * 30)));
                döviz1.Text = "" + ekranaGidecekler[0] + " adet ";
                döviz1.Width = 150;
                form1.Controls.Add(döviz1);

                Label döviz2 = new Label();
                döviz2.Location = new Point(chart.Size.Width , (110 + (distance * 30)));
                döviz2.Text = "" + ekranaGidecekler[1] + " adet ";
                döviz2.Width = 150;
                form1.Controls.Add(döviz2);

                Label döviz3 = new Label();
                döviz3.Location = new Point(chart.Size.Width , (140 + (distance * 30)));
                döviz3.Text = ""+ ekranaGidecekler[2] + " adet ";
                döviz3.Width = 150;
                form1.Controls.Add(döviz3);
            };

            Application.Run(form1);
        }

        #region Methods
        public static double kazandiranlariBul(int kazandiran, List<double[]> Kurlar)
        {
            int sayac = 0;
            double kazandiranKurlarinSonHali = new double();
            foreach (var item in Kurlar)
            {
                if (kazandiran == sayac)
                {
                    kazandiranKurlarinSonHali = item.Last();
                }
                sayac++;
            }
            return kazandiranKurlarinSonHali;
        }       // Kazanım eğiliminde olan dövizleri veren fonksiyonumuz.

        public static void dogruEgimi(List<double[]> Kurlar, double[] egimler)
        {
            int sayac1 = 0;
            foreach (var item in Kurlar)
            {
                double xOrt = 0;
                double yOrt = 0;
                int x1 = 0;
                int x2 = 0;

                foreach (var kurDegerleri in item)
                {
                    xOrt += x1;
                    yOrt += kurDegerleri;
                    x1++;
                }

                xOrt = xOrt / item.Length;
                yOrt = yOrt / item.Length;

                double payToplam = 0;
                double paydaToplam = 0;

                foreach (var kurDegerleri in item)
                {
                    payToplam += (x2 - xOrt) * (kurDegerleri - yOrt);
                    paydaToplam += Math.Pow(x2 - xOrt, 2);
                    x2++;
                }

                double a = payToplam / paydaToplam;

                double egim = Math.Round(a, 4);

                egimler[sayac1] = egim;
                sayac1++;
            }

        }                              // Eğimleri hesaplayan fonksiyonumuz.

        public static void xmlOku(List<double[]> Kurlar, List<string> kurAdlari)
        {
            XmlNodeType type;
            List<double> xmlKurlari = new List<double>();               // Kurların geçici olarak tutulduğu listemiz.

            XmlTextReader reader = new XmlTextReader("book.xml");       // Okunacak xml adı. Yeri bin/debug.

            while (reader.Read())
            {
                type = reader.NodeType;

                if (type == XmlNodeType.Element)
                {

                    if (reader.Name == "title")
                    {
                        reader.Read();
                        kurAdlari.Add(reader.Value);                    // Kurların adları burada listeye atılıyor.
                    }

                    if (reader.Name == "price")
                    {
                        reader.Read();
                        xmlKurlari.Add(Convert.ToDouble(reader.Value)); // Kur değerleri burada geçici listeye atılıyor
                    }
                }

                if (type == XmlNodeType.EndElement)
                {
                    if (reader.Name == "kur")
                    {
                        double[] kur = xmlKurlari.ToArray();
                        Kurlar.Add(kur);                                // Kurlarımızın asıl tutulduğu diziye kur değerleri burada atılıyor.
                        xmlKurlari.ToList();
                        xmlKurlari.Clear();                             // Geçici liste temizleniyor ki bir sonraki adımlarda tekrar kullanılabilsin.
                    }
                }
            }
            reader.Close();
        }

        public static void restart()
        {
            Application.Restart();
            //System.Timers.Timer tmr = new System.Timers.Timer();                                // Uygulamayı belirli aralıklarla restart eden kod parçası.
            //tmr.Interval = 10000;
            //tmr.Start();
            //tmr.Elapsed += (s, e) =>
            //{
            //    Application.Restart();
            //};
        }

        public static void seriesOlustur(double[] egimler, Chart chart, List<string> kurAdlari, List<double[]> Kurlar)
        {
            int sayac2 = 1;
            int sayac9 = 0;

            foreach (var item in egimler)
            {
                var series1 = new Series();
                series1.Name = "Series" + sayac2 + "";
                series1.ChartType = SeriesChartType.Line;
                series1.XValueType = ChartValueType.Int32;
                chart.Series.Add(series1);
                chart.Series["Series" + sayac2 + ""].Points.AddXY(0, 0);

                double y;

                if (item < 0)                                                     // Eğim sıfırdan küçükse, bazı işlemler yapılır.
                {
                    y = Math.Abs(item) * 50;
                    chart.Series["Series" + sayac2 + ""].Points.AddXY(-50, y);
                    chart.Series["Series" + sayac2 + ""].Color = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                }
                else                                                            // Eğim sıfırdan büyükse bu işlemler yapılır.
                {
                    y = item * 50;
                    chart.Series["Series" + sayac2 + ""].Points.AddXY(50, y);
                    chart.Series["Series" + sayac2 + ""].Color = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                }

                chart.Series["Series" + sayac2 + ""].LegendText = kurAdlari[sayac9] + " eğrisi";
                chart.Series["Series" + sayac2 + ""].BorderWidth = 3;
                sayac2++;
                sayac9++;
            }

            chart.Legends.Add(new Legend());
            int a = chart.Series.Count() + 1;
            int sayac10 = 0;
            int sayac3 = 1;

            foreach (var item in Kurlar)
            {
                var kur = new Series();
                kur.Name = "Series" + a + "";
                kur.ChartType = SeriesChartType.Point;
                kur.XValueType = ChartValueType.Double;
                chart.Series.Add(kur);
                chart.Series["Series" + a + ""].MarkerStyle = MarkerStyle.Circle;

                for (int i = 0; i < item.Length; i++)
                {
                    chart.Series["Series" + a + ""].Points.AddXY(i, item[i]);
                }

                chart.Series["Series" + a + ""].LegendText = kurAdlari[sayac10] + " kuru";
                chart.Series["Series" + a + ""].Color = chart.Series["Series" + sayac3 + ""].Color;

                sayac3++;
                sayac10++;
                a++;
            }
        }

        public static int birimBasinaKazandirma(double[] egimler, Form form1, int uzaklik, List<string> kurAdlari, Chart chart, List<double> BackpackeGidecekVeriler)
        {
            int sayac7 = 0;
            int sayac8 = 0;
            for (int i = 0; i < egimler.Length; i++)                        // Kurların eğimlerine göre sağdaki labelların oluşturulması. 1 birim başına kazançlar kısmı.
            {
                if (egimler[i] > 0)                                         // Eğimi sıfırdan büyük olanların gösterimlerinin konfigüre edildiği yer.
                {
                    Label dovizAdiLabel = new Label();
                    Label dovizEgimLabel = new Label();
                    dovizEgimLabel.BackColor = System.Drawing.Color.FromArgb(100, 255, 100);
                    dovizEgimLabel.Location = new Point(chart.Size.Width + 55, (50 + (uzaklik * 30)));
                    dovizAdiLabel.Location = new Point(chart.Size.Width, (50 + (uzaklik * 30)));
                    dovizAdiLabel.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                    dovizAdiLabel.Size = new System.Drawing.Size(50, 23);
                    dovizAdiLabel.Text = kurAdlari[i] + ":";
                    dovizEgimLabel.Text = Convert.ToString(egimler[i]);

                    form1.Controls.Add(dovizAdiLabel);
                    form1.Controls.Add(dovizEgimLabel);

                    uzaklik++;
                    sayac8++;

                }
                else if (egimler[i] < 0)                                    // Eğimi sıfırdan küçük olanların gösterimlerinin konfigüre edildiği yer.
                {
                    Label dovizAdiLabel = new Label();
                    Label dovizEgimLabel = new Label();
                    dovizEgimLabel.BackColor = System.Drawing.Color.FromArgb(255, 80, 80);
                    dovizEgimLabel.Location = new Point(chart.Size.Width + 55, (50 + (uzaklik * 30)));
                    dovizAdiLabel.Location = new Point(chart.Size.Width, (50 + (uzaklik * 30)));
                    dovizAdiLabel.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                    dovizAdiLabel.Size = new System.Drawing.Size(50, 23);
                    dovizAdiLabel.Text = kurAdlari[i] + ":";
                    dovizEgimLabel.Text = Convert.ToString(egimler[i]);

                    form1.Controls.Add(dovizAdiLabel);
                    form1.Controls.Add(dovizEgimLabel);

                    uzaklik++;
                }
                else                                                        // Eğimi sıfır olanların gösterimlerinin konfigüre edildiği yer.
                {
                    Label dovizAdiLabel = new Label();
                    Label dovizEgimLabel = new Label();
                    dovizEgimLabel.BackColor = System.Drawing.Color.FromArgb(250, 250, 150);
                    dovizEgimLabel.Location = new Point(chart.Size.Width + 55, (50 + (uzaklik * 30)));
                    dovizAdiLabel.Location = new Point(chart.Size.Width, (50 + (uzaklik * 30)));
                    dovizAdiLabel.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                    dovizAdiLabel.Size = new System.Drawing.Size(50, 23);
                    dovizAdiLabel.Text = kurAdlari[i] + ":";
                    dovizEgimLabel.Text = Convert.ToString(egimler[i]);

                    form1.Controls.Add(dovizAdiLabel);
                    form1.Controls.Add(dovizEgimLabel);

                    uzaklik++;
                }
            }

            for (int i = 0; i < egimler.Length; i++)
            {
                if (egimler[i] > 0)
                {
                    BackpackeGidecekVeriler.Add(egimler[i]);
                    sayac7++;
                }
            }
            return uzaklik;
        }

        public static void enCokKazandiranlariGoster(double[] egimler, Chart chart, Form form1, List<string> kurAdlari, List<int> kazandıranKurBilgileri)
        {
            int uzaklik2 = 0;
            for (int i = 0; i < egimler.Length; i++) ///  En çok kazandıranları gösteriyor.
            {

                if (egimler[i] > 0)
                {
                    kazandıranKurBilgileri.Add(i);

                    Label enCokKazandiranDovizAdiLabel = new Label();
                    Label enCokKazandiranDovizEgimLabel = new Label();
                    uzaklik2++;
                    enCokKazandiranDovizEgimLabel.BackColor = System.Drawing.Color.FromArgb(100, 255, 100);
                    enCokKazandiranDovizEgimLabel.Location = new Point(chart.Size.Width + 255, (50 + (uzaklik2 * 30)));
                    enCokKazandiranDovizAdiLabel.Location = new Point(chart.Size.Width + 200, (50 + (uzaklik2 * 30)));
                    enCokKazandiranDovizAdiLabel.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                    enCokKazandiranDovizAdiLabel.Size = new System.Drawing.Size(50, 23);
                    enCokKazandiranDovizAdiLabel.Text = kurAdlari[i] + ":";
                    enCokKazandiranDovizEgimLabel.Text = Convert.ToString(egimler[i]);

                    form1.Controls.Add(enCokKazandiranDovizAdiLabel);
                    form1.Controls.Add(enCokKazandiranDovizEgimLabel);
                }
            }
        }

        public static void kazandiranlarinBilgileri(List<int> kazandıranKurBilgileri, double[] kazandiranlarinSonKurlari, int[] kazananKurIndisleri, List<double[]> Kurlar, Chart chart, List<string> kurAdlari, Form form1)
        {
            int uzaklik3 = 0;
            int sayac4 = 0;
            int sayac5 = 0;
            int sayac6 = 0;

            foreach (var item in kazandıranKurBilgileri)
            {
                kazananKurIndisleri[sayac6] = item;
                sayac6++;
            }

            foreach (var item in kazandıranKurBilgileri)
            {
                kazandiranlarinSonKurlari[sayac5] = kazandiranlariBul(item, Kurlar);
                sayac5++;
            }

            for (int i = 0; i < kazandiranlarinSonKurlari.Length; i++)
            {


                Label enCokKazandiranDovizAdiLabel2 = new Label();
                Label enCokKazandiranDovizKurLabel2 = new Label();
                uzaklik3++;
                enCokKazandiranDovizKurLabel2.BackColor = System.Drawing.Color.FromArgb(100, 255, 100);
                enCokKazandiranDovizKurLabel2.Location = new Point(chart.Size.Width + 455, (50 + (uzaklik3 * 30)));
                enCokKazandiranDovizAdiLabel2.Location = new Point(chart.Size.Width + 400, (50 + (uzaklik3 * 30)));
                enCokKazandiranDovizAdiLabel2.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                enCokKazandiranDovizAdiLabel2.Size = new System.Drawing.Size(50, 23);
                enCokKazandiranDovizAdiLabel2.Text = kurAdlari[kazananKurIndisleri[i]] + ":";
                enCokKazandiranDovizKurLabel2.Text = Convert.ToString(kazandiranlarinSonKurlari[sayac4]);
                sayac4++;

                form1.Controls.Add(enCokKazandiranDovizAdiLabel2);
                form1.Controls.Add(enCokKazandiranDovizKurLabel2);
            }
        }

        public static double[,] populasyonUretme(double anaPara, int nufusBuyuklugu=1, int kromozomUzunlugu=3)
        {
            double[,] nufus=new double[nufusBuyuklugu,kromozomUzunlugu];
            Random dovizAdet = new Random();
            for (int i = 0; i < nufusBuyuklugu; i++)
            {
                for (int j = 0; j < kromozomUzunlugu; j++)
                {
                    nufus[i, j] = dovizAdet.Next(Convert.ToInt32(anaPara/(nufusBuyuklugu*5)), Convert.ToInt32(anaPara/2));
                }
            }
            return nufus;
        }

        public static double[] amacFNDegeri(double[,] nufus, double[] kazandiranlarinSonKurlari, int nufusBuyuklugu = 1, int kromozomUzunlugu = 3)
        {
            double[] amacFN = new double[nufusBuyuklugu];
            for (int i = 0; i < nufusBuyuklugu; i++)
            {
                for (int j = 0; j < kromozomUzunlugu; j++)
                {
                    amacFN[i]=nufus[i,j]*kazandiranlarinSonKurlari[j];
                }
            }
            return amacFN;
        }

        public static float[] uygunlukAta(double[] amacFN)
        {
            double[] temp = new double[amacFN.Length];
            int[] amacFNindisleri = new int[amacFN.Length];
            float[] uygunlukDegerleri = new float[amacFN.Length];
            int uMax = 1;
            for (int i = 0; i < amacFN.Length; i++)
            {
                temp[i] = amacFN[i];
            }


            int s=0;
            Array.Sort(amacFN);
            Array.Reverse(amacFN);

            for (int i = 0; i < amacFN.Length; i++)
            {
                for (int j = 0; j < amacFN.Length; j++)
                {
                    if (amacFN[i]==temp[j])
                    {
                        amacFNindisleri[s] = j;
                    }
                }
                s++;
            }
            float N = amacFN.Length;
            for (int i = 0; i < amacFN.Length; i++)
            {
               uygunlukDegerleri[i]  = uMax*((N-i-1)/(N-1));
            }
            return uygunlukDegerleri;
        }

        public static int[] secimIslemi(float[] uygunlukDegerleri)
        {
            float[] ruletUygunluk = new float[uygunlukDegerleri.Length];
            float toplamUygunluk = uygunlukDegerleri.Sum();
            int[] yeniJenerasyonIndisleri = new int[uygunlukDegerleri.Length];
            for (int i = 0; i < uygunlukDegerleri.Length; i++)
            {
                ruletUygunluk[i] = uygunlukDegerleri[i] / toplamUygunluk;
            }

            Random rnd = new Random();
            for (int i = 0; i < uygunlukDegerleri.Length; i++)
            {
                double sayi = rnd.NextDouble();
                int dilim = 0;
                float temp = 0;
                int sayac = 0;
                while (sayi>temp)
                {
                    dilim = dilim + 1;
                    temp = temp + ruletUygunluk[sayac];
                    sayac++;
                }
                yeniJenerasyonIndisleri[i] = dilim;
            }
            return yeniJenerasyonIndisleri;
        }

        public static double[,] caprazlama(int[] yeniJenerasyonIndisleri, double[,] nufus)
        {
            double[,] ebeveynler = new double[nufus.GetLength(0),nufus.GetLength(1)];
            double[,] cocuklar = new double[nufus.GetLength(0), nufus.GetLength(1)];

            for (int i = 0; i < ebeveynler.GetLength(0); i++)
            {
                for (int j = 0; j < ebeveynler.GetLength(1); j++)
                {
                    ebeveynler[i, j] = nufus[yeniJenerasyonIndisleri[i] - 1,j];
                }
            }

            for (int i = 0; i < ebeveynler.GetLength(0); i++)
            {
                if (i < ebeveynler.GetLength(0)/2 )
                {
                    cocuklar[i, 0] = ebeveynler[i, 0];
                    cocuklar[i, 1] = ebeveynler[ebeveynler.GetLength(0) / 2 + i, 1];
                    cocuklar[i, 2] = ebeveynler[ebeveynler.GetLength(0) / 2 + i, 2];
                }
                else
                {
                    cocuklar[i, 0] = ebeveynler[i, 0];
                    cocuklar[i, 1] = ebeveynler[Math.Abs(ebeveynler.GetLength(0) / 2 - i), 1];
                    cocuklar[i, 2] = ebeveynler[Math.Abs(ebeveynler.GetLength(0) / 2 - i), 2];
                }
            }
            return cocuklar;
        }

        public static double[,] mutasyon(double[,] cocuklar)
        {
            Random rnd = new Random();
            double[] mutasyonOranlari = new double[cocuklar.Length];
            for (int i = 0; i < cocuklar.Length; i++)
            {
                mutasyonOranlari[i] = rnd.NextDouble();
            }
            for (int i = 0; i < cocuklar.Length; i++)
            {
                if (mutasyonOranlari[i] < 0.05)
                {
                    cocuklar[i / 3, i % 3] += 20;
                }
            }
            return cocuklar;
        }

        public static double[,] iterator(double[,] cocuklar, double[] kazandiranlarinSonKurlari)
        {


            for (int i = 0; i < 50; i++)
            {
                var y = amacFNDegeri(cocuklar, kazandiranlarinSonKurlari, cocuklar.GetLength(0), cocuklar.GetLength(1));
                var z = uygunlukAta(y);
                var t = secimIslemi(z);
                var u = caprazlama(t, cocuklar);
                var v = mutasyon(u);
                cocuklar = v;
            }
            return cocuklar;
        }

        public static double[] optimum(double[,] sonNesil, double[] sonKurlar)
        {
            var sonDegerler = amacFNDegeri(sonNesil,sonKurlar,sonNesil.GetLength(0),sonNesil.GetLength(1));
            double[] best = new double[sonNesil.GetLength(0)];
            double[] final = new double[sonNesil.GetLength(1)];
            for (int i = 0; i < sonDegerler.Length; i++)
            {
                best[i] = sonDegerler[i];
            }

            Array.Sort(sonDegerler);
            Array.Reverse(sonDegerler);
            for (int i = 0; i < sonDegerler.Length; i++)
            {
                if (sonDegerler[0] == best[i])
                {
                    for (int j = 0; j < final.Length; j++)
                    {
                        final[j] = sonNesil[i, j];
                    }
                }
            }
            return final;
        }

        #endregion
    }
}