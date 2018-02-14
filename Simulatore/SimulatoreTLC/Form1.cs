using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimulatoreTLC
{
    public partial class Form1 : Form
    {
        static string percorso = "";
        static TimeSpan durataSimulazione = new TimeSpan(0, 0, 0);

        //variabili della simulazione
        static double NUTENTI = 1000000; // numero di utenti del sistema 
        static double Y = 1000000; // massima dimensione della coda 
        static double lambda; // tasso di nascita degli utenti 
        static double MU = 1; // tasso di morte degli utenti 
        static double INCR_LAMBDA = 0.01; // incremento sul tasso di nascita degli utenti
        static int x; // contatore del numero di utenti attualmente in servizio 
        static int q; // contatore del numero di utenti attualmente in coda 
        static int k; // contatore del numero di utenti attualmente nel sistema
        static double accumula_tx; // accumula il tempo trascorso in servizio 
        static double accumula_tq; // accumula il tempo trascorso in coda 
        static double accumula_tk; // accumula il tempo trascorso nel sistema
        static double accumula_x; // accumula il numero di utenti attualmente in servizio 
        static double accumula_q; // accumula il numero di utenti attualmente in coda
        static double accumula_k; // accumula il numero di utenti attualmente nel sistema
        static double nUtEntratiServ; // contatore del numero di utenti entrati in servizio 
        static double nUtEntratiCoda; // accumula il numero di utenti entrati in coda 
        static double nUtEntratiSist; // accumula il numero di utenti entrati nel sistema
        static double nUtUscitiServ; // contatore del numero di utenti usciti dal servizio 
        static double nUtUscitiCoda; // accumula il numero di utenti usciti in coda 
        static double nUtUscitiSist; // accumula il numero di utenti usciti nel sistema
        static double nUtentiGenerati; // contatore del numero di utenti generati 
        static double nUtentiPersi; // contatore del numero di utenti persi per coda piena 
        static double occorrenzeCodaPiena; // numero di volte in cui la coda è piena
        static double tInterarrivo; // tempo trascorso fra due arrivi consecutivi di utenti 
        static double tSimulazione; // accumula il tempo di simulazione 
        static double tServitoreLibero; // accumula il tempo in cui il servitore è libero 
        static double tServitoreOccupato; // accumula il tempo in cui il servitore è occupato
        static Random r;
        static List<Utente> coda;
        static Utente utente;

        // genera un tempo di interarrivo distribuito secondo Poisson
        public static double poisson()
        {
            return (-Math.Log(1 - r.NextDouble()) / lambda);
        }

        // genera un tempo di interservizio distribuito secondo Poisson
        public static double txUtente()
        {

            return (-Math.Log(1 - r.NextDouble()) / MU);
        }

        public static void inizializzaSimulazione()
        {
            r = new Random(DateTime.Now.Millisecond);
            x = 0; // contatore del numero di utenti attualmente in servizio 
            q = 0; // contatore del numero di utenti attualmente in coda 
            k = 0; // contatore del numero di utenti attualmente nel sistema
            accumula_tx = 0; // accumula il tempo trascorso in servizio 
            accumula_tq = 0; // accumula il tempo trascorso in coda 
            accumula_tk = 0; // accumula il tempo trascorso nel sistema
            accumula_x = 0; // accumula il numero di utenti attualmente in servizio 
            accumula_q = 0; // accumula il numero di utenti attualmente in coda

            accumula_k = 0; // accumula il numero di utenti attualmente nel sistema
            nUtEntratiServ = 0; // contatore del numero di utenti entrati in servizio 
            nUtEntratiCoda = 0; // accumula il numero di utenti entrati in coda 
            nUtEntratiSist = 0; // accumula il numero di utenti entrati nel sistema
            nUtUscitiServ = 0; // contatore del numero di utenti usciti dal servizio 
            nUtUscitiCoda = 0; // accumula il numero di utenti usciti in coda 
            nUtUscitiSist = 0; // accumula il numero di utenti usciti nel sistema
            nUtentiGenerati = 0; // contatore del numero di utenti generati 
            nUtentiPersi = 0; // contatore del numero di utenti persi per coda piena 
            occorrenzeCodaPiena = 0; // numero di volte in cui la coda è piena
            tInterarrivo = 0; // tempo trascorso fra due arrivi consecutivi di utenti
            tSimulazione = 0; // accumula il tempo di simulazione 
            tServitoreLibero = 0; // accumula il tempo in cui il servitore è libero 
            tServitoreOccupato = 0; // accumula il tempo in cui il servitore è occupato
            coda = new List<Utente>();
            utente = new Utente();

        }

        // gestisce l’accodamento di un utente in coda alla lista
        public static void inserisciInCoda(Utente utente)
        {
            coda.Add(utente);
        }

        // ritorna l’elemento di testa della lista, cancellandolo. // il chiamante si è già assicurato che la coda NON sia vuota 
        public static Utente prelevaInTesta()
        {
            Utente u = coda[0];
            coda.RemoveAt(0);
            return u;
        }

        // aggiunge agli utenti in coda il tempo time 
        public static void aggiorna_tq_tkCoda(double time)
        {
            foreach (var u in coda)
            {
                u.TempoInCoda += time;
            }
        }

        // gestisce l’entrata in servizio dell’utente 
        public static bool serviUtente(ref Utente utente, ref int x, ref double nUtEntratiServ)
        {
            // incremento il contatore degli utenti entrati in servizio 
            nUtEntratiServ++;
            x++; // l’utente entra in servizio 
            utente.TempoServizio = txUtente();
            // starà in servizio per un certo tempo txUtente 
            utente.TempoServizioRimanente = utente.TempoServizio; // setto il tempo di servizio rimanente
            return true; // l’utente arrivato è stato gestito
        }

        // gestisce l’entrata in servizio di un utente quando il sistema è vuoto 
        public static bool gestisciArrivoUtenteConSistemaVuoto(ref Utente utente, ref int x, ref int k, ref double nUtEntratiServ, ref double nUtEntratiSist)
        {
            // incremento il contatore degli utenti entrati nel sistema 
            nUtEntratiSist++;
            // accumulo il numero di utenti attualmente nel sistema 
            accumula_k = accumula_k + k;
            // accumulo il numero di utenti attualmente in coda 
            accumula_q = accumula_q + q;
            k++;
            // arriva l’utente che entra nel sistema 
            utente.TempoServizio = 0;// non è ancora stato messo in servizio 
            utente.TempoInCoda = 0; // non ha atteso in coda perchè il sistema è vuoto 
            utente.TempoNelSistema = 0; // è appena entrato nel sistema 
            utente.TempoServizioRimanente = 0; // non è ancora stato messo in servizio
            // gestisce l’entrata in servizio dell’utente 
            return (serviUtente(ref utente, ref x, ref nUtEntratiServ));
        }

        // gestisce l’eventuale (MM1Y) accodamento dell’utente
        public static bool gestisciAccodamentoUtenteMM1Y(ref int q, ref int k, ref double nUtEntratiCoda, ref double nUtEntratiSist, ref double nUtentiPersi)
        {
            Utente utDaAccodare = new Utente();
            // se la coda NON è piena 
            if (q < Y)
            {
                // incremento il contatore del numero totale di utenti entrati nel sistema
                nUtEntratiSist++;
                // incremento il contatore del numero totale di utenti entrati in coda 
                nUtEntratiCoda++;
                // accumulo il numero di utenti attualmente nel sistema 
                accumula_k = accumula_k + k;
                // accumulo il numero di utenti attualmente in coda 
                accumula_q = accumula_q + q;
                k++; // arriva l’utente che entra nel sistema 
                q++; // incremento il numero di utenti in coda 
                utDaAccodare.TempoServizio = 0; // non è ancora stato messo in servizio 
                utDaAccodare.TempoInCoda = 0; // entrerà ora in coda 
                utDaAccodare.TempoNelSistema = 0; // è appena entrato nel sistema 
                utDaAccodare.TempoServizioRimanente = 0; // non è ancora stato messo in servizio 
                inserisciInCoda(utDaAccodare); // gestisce l’accodamento
                                               // se la coda è piena aggiorno il contatore di coda piena 
                if (q == Y)
                    occorrenzeCodaPiena++;
            } // se la coda è piena 
            else
                nUtentiPersi++;// arriva l’utente che trovando la coda piena sarà perso 
            return true; // l’utente arrivato è stato gestito
        }

        // gestisce l’uscita dell’utente attualmente in servizio 
        public static void gestisciUscitaUtente(Utente utente, ref int x, ref int k, ref double nUtUscitiServ, ref double nUtUscitiSist)
        {
            // incremento il contatore del numero totale di utenti usciti dal servizio
            nUtUscitiServ++;
            // incremento il contatore del numero totale di utenti usciti dal sistema 
            nUtUscitiSist++;
            // accumulo i tempi di servizio 
            accumula_tx = accumula_tx + utente.TempoServizio;
            // accumulo i tempi di permanenza nel sistema 
            accumula_tk = accumula_tk + utente.TempoNelSistema;
            x--; // l’utente esce dal servizio 
            k--; // ed esce dal sistema
        }

        // gestisce l’entrata in servizio del primo utente in coda (FIFO) 
        public static void gestisciServiUtenteInCoda(ref Utente utente, ref int x, ref int q, ref double nUtEntratiServ, ref double nUtUscitiCoda)
        {
            // incremento il contatore del numero totale di utenti usciti dalla coda 
            nUtUscitiCoda++;
            // accumulo i tempi di attesa in coda
            accumula_tq = accumula_tq + utente.TempoInCoda;
            q--; // l’utente esce dalla coda
            utente = prelevaInTesta(); // prelevo il primo utente dalla coda 
                                       // gestisco l’entrata in servizio dell’utente 
            serviUtente(ref utente, ref x, ref nUtEntratiServ);
        }

        public static void simulazione()
        {
            bool arrivato; // 1 se è arrivato un utente, 0 se NON è arrivato alcun utente
            // arriva il primo utente che trova il sistema vuoto 
            arrivato = gestisciArrivoUtenteConSistemaVuoto(ref utente, ref x, ref k, ref nUtEntratiServ, ref nUtEntratiSist);
            nUtentiGenerati = 0;
            while (nUtentiGenerati < NUTENTI)
            {
                // se l’utente è già arrivato 
                if (arrivato)
                {
                    nUtentiGenerati++; // genero un arrivo 
                                       // il prossimo utente arriverà fra un tempo tInterarrivo
                    tInterarrivo = poisson();
                    arrivato = false; // poichè NON è ancora arrivato 
                }
                // se il sistema è vuoto
                if (k == 0)
                {
                    // la simulazione avanza temporalmente fino all’istante di arrivo 
                    tSimulazione = tSimulazione + tInterarrivo;
                    // accumulo il tempo di servitore libero 
                    tServitoreLibero = tServitoreLibero + tInterarrivo;
                    // arriva l’utente che trova il sistema vuoto 
                    arrivato = gestisciArrivoUtenteConSistemaVuoto(ref utente, ref x, ref k, ref nUtEntratiServ, ref nUtEntratiSist);
                } // se il sistema NON è vuoto 
                else
                {
                    // se l’utente arriverà prima che termini il servizio di quello attualmente in servizio 
                    if (tInterarrivo < utente.TempoServizioRimanente)
                    {
                        // la simulazione avanza temporalmente fino all’istante di arrivo 
                        tSimulazione = tSimulazione + tInterarrivo;
                        // trascorre il tempo tInterarrivo per l’utente in servizio 
                        utente.TempoNelSistema = utente.TempoNelSistema + tInterarrivo;
                        // aggiorno il tempo di servizio rimanente 
                        utente.TempoServizioRimanente = utente.TempoServizioRimanente - tInterarrivo;
                        // trascorre il tempo tInterarrivo per gli EVENTUALI utenti in coda 
                        aggiorna_tq_tkCoda(tInterarrivo);
                        // gestisco l’eventuale (MM1Y) accodamento dell’utente 
                        arrivato = gestisciAccodamentoUtenteMM1Y(ref q, ref k, ref nUtEntratiCoda, ref nUtEntratiSist, ref nUtentiPersi);
                    } // se l’utente arriverà dopo che termini il servizio di quello attualmente in servizio 
                    else
                    {
                        // la simulazione avanza temporalmente fino all’istante di uscita 
                        tSimulazione = tSimulazione + utente.TempoServizioRimanente;
                        // trascorre il tempo txRimanente per l’utente in servizio 
                        utente.TempoNelSistema = utente.TempoNelSistema + utente.TempoServizioRimanente;
                        // trascorre il tempo txRimanente per gli EVENTUALI utenti in coda 
                        aggiorna_tq_tkCoda(utente.TempoServizioRimanente);
                        // trascorre il tempo txRimanente per l’utente in arrivo 
                        tInterarrivo = tInterarrivo - utente.TempoServizioRimanente;
                        // gestisce l’uscita dell’utente attualmente in servizio 
                        gestisciUscitaUtente(utente, ref x, ref k, ref nUtUscitiServ, ref nUtUscitiSist);
                        // se c’è coda 
                        if (q > 0)
                        {
                            // gestisce l’entrata in servizio del primo utente in coda (FIFO) 
                            gestisciServiUtenteInCoda(ref utente, ref x, ref q, ref nUtEntratiServ, ref nUtUscitiCoda);
                        }
                    }// se l’utente arriverà dopo che termini il servizio di quello attualmente in servizio 
                }// se il sistema NON è vuoto 
            }// while (nUtentiGenerati < NUTENTI)
        }

        public static void stampaRisultatiSimulati(string percorso, List<double> r, List<double> tq, List<double> tk, List<double> k, List<double> q, List<double> x)
        {


            StreamWriter ftempoCoda = new StreamWriter(percorso + "\\simulati\\tempoCoda.txt", false);
            StreamWriter ftempoSistema = new StreamWriter(percorso + "\\simulati\\tempoSistema.txt", false);
            StreamWriter fUtentiCoda = new StreamWriter(percorso + "\\simulati\\utentiCoda.txt", false);
            StreamWriter fUtentiSistema = new StreamWriter(percorso + "\\simulati\\utentiSistema.txt", false);

            for (int i = 0; i < tq.Count; i++)
            {

                ftempoCoda.Write(r[i].ToString().Replace(',', '.'));
                ftempoCoda.Write(' ');
                ftempoCoda.Write(tq[i].ToString().Replace(',', '.'));
                ftempoCoda.WriteLine();

                ftempoSistema.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), tk[i].ToString().Replace(',', '.'));

                fUtentiSistema.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), k[i].ToString().Replace(',', '.'));

                fUtentiCoda.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), q[i].ToString().Replace(',', '.'));


            }


            ftempoCoda.Close();
            ftempoSistema.Close();
            fUtentiCoda.Close();
            fUtentiSistema.Close();

        }


        public static void teoricoMM1(out double teoricoUtentiSistema, out double teoricoUtentiCoda, out double teoricoTempoCoda, out double teoricoTempoSistema, out double teoricoTempoServizio)
        {
            double rho = lambda / (double)MU;
            teoricoUtentiSistema = rho / (1 - rho);
            teoricoUtentiCoda = Math.Pow(rho, 2) / (1 - rho);
            teoricoTempoSistema = teoricoUtentiSistema * (1 / lambda);
            teoricoTempoServizio = 1 / MU;
            teoricoTempoCoda = teoricoTempoSistema - teoricoTempoServizio;
        }

        public static void teoricoMM1Y(out double teoricoUtentiSistema, out double teoricoUtentiCoda, out double teoricoTempoCoda, out double teoricoTempoSistema, out double teoricoTempoServizio)
        {
            double a = lambda / (double)MU;
            double py = ((1 - a) / (1 - Math.Pow(a, Y + 1))) * Math.Pow(a, Y);
            double lambdaMedio = lambda * (1 - py);
            teoricoUtentiSistema = (a / (1 - a)) - (((Y + 1) * Math.Pow(a, Y + 1)) / (1 - Math.Pow(a, Y + 1)));
            teoricoUtentiCoda = (Math.Pow(a, 2) / (1 - a)) - (((Y + 1) * Math.Pow(a, Y + 1)) / (1 - Math.Pow(a, Y + 1)));
            teoricoTempoSistema = teoricoUtentiSistema / lambdaMedio;
            teoricoTempoServizio = 1 / MU;
            teoricoTempoCoda = teoricoUtentiCoda / lambdaMedio;
        }

        public static void stampaTeorici(string percorso, List<double> r, List<double> tq, List<double> tk, List<double> k, List<double> q, List<double> x)
        {
          

            StreamWriter fTtempoCoda = new StreamWriter(percorso + "\\teorici\\tempoCoda.txt", false);
            StreamWriter fTtempoSistema = new StreamWriter(percorso + "\\teorici\\tempoSistema.txt", false);
            StreamWriter fTUtentiCoda = new StreamWriter(percorso + "\\teorici\\utentiCoda.txt", false);
            StreamWriter fTUtentiSistema = new StreamWriter(percorso + "\\teorici\\utentiSistema.txt", false);

            for (int i = 0; i < tq.Count; i++)
            {

                fTtempoCoda.Write(r[i].ToString().Replace(',', '.'));
                fTtempoCoda.Write(' ');
                fTtempoCoda.Write(tq[i].ToString().Replace(',', '.'));
                fTtempoCoda.WriteLine();

                fTtempoSistema.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), tk[i].ToString().Replace(',', '.'));

                fTUtentiSistema.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), k[i].ToString().Replace(',', '.'));

                fTUtentiCoda.WriteLine("{0} {1}", r[i].ToString().Replace(',', '.'), q[i].ToString().Replace(',', '.'));




            }

            fTtempoCoda.Close();
            fTtempoSistema.Close();
            fTUtentiCoda.Close();
            fTUtentiSistema.Close();

        }

        public static void CalcolaErrore(List<double> stk, List<double> stq, List<double> sk, List<double> sq, List<double> ttk, List<double> ttq, List<double> tk, List<double> tq, out List<double> erroreTk, out List<double> erroreTq, out List<double> erroreK, out List<double> erroreQ)
        {
            erroreK = new List<double>();
            erroreQ = new List<double>();
            erroreTk = new List<double>();
            erroreTq = new List<double>();

            for (int i = 0; i < sk.Count; i++)
            {

                erroreK.Add((Math.Abs(sk[i] - tk[i]) / tk[i]) * 100);
            }

            for (int i = 0; i < stk.Count; i++)
            {
                erroreTk.Add((Math.Abs(stk[i] - ttk[i]) / ttk[i]) * 100);
            }

            for (int i = 0; i < stq.Count; i++)
            {
                erroreTq.Add((Math.Abs(stq[i] - ttq[i]) / tk[i]) * 100);
            }

            for (int i = 0; i < sq.Count; i++)
            {
                erroreQ.Add((Math.Abs(sq[i] - tq[i]) / tq[i]) * 100);
            }

        }

        public static string CreaCartelle(string percorso)
        {
            string newPath;
            if(Directory.Exists(percorso + "\\risultati"))
            {
                int i = 0;
                do
                {
                    i++;
                } while (Directory.Exists(percorso + "\\risultati(" + i.ToString() + ")"));
                percorso = percorso + "\\risultati(" + i.ToString() + ")";
                Directory.CreateDirectory(percorso + "\\errore");
                Directory.CreateDirectory(percorso + "\\teorici");
                Directory.CreateDirectory(percorso + "\\simulati");
                Directory.CreateDirectory(percorso + "\\grafici");
                Directory.CreateDirectory(percorso + "\\script");
            }
            else
            {
                Directory.CreateDirectory(percorso + "\\risultati\\errore");
                Directory.CreateDirectory(percorso + "\\risultati\\teorici");
                Directory.CreateDirectory(percorso + "\\risultati\\simulati");
                Directory.CreateDirectory(percorso + "\\risultati\\grafici");
                Directory.CreateDirectory(percorso + "\\risultati\\script");
                percorso = percorso + "\\risultati";
            }
            newPath = percorso;
            return newPath;



        }

        public static double TrovaMax(List<double> lista)
        {
            double max = 0;
            foreach(var i in lista)
            {
                if (max < i)
                    max = i;
            }
            return max;
        }

        public static double TrovaMax(double a, double b)
        {
            if (a < b)
                return b;
            else
                return a;
        }
        public static void CreaScript(List<double> erroreTq, List<double> erroreTk, List<double> erroreK, List<double> erroreQ, List<double> teoricoTq, List<double> teoricoTk, List<double> teoricoQ, List<double> teoricoK, List<double> simulatoTk, List<double> simulatoTq, List<double> simulatoK, List<double> simulatoQ)
        {


            //massimi errori
            double maxETq = TrovaMax(erroreTq);
            double maxETk = TrovaMax(erroreTk);
            double maxEQ = TrovaMax(erroreQ);
            double maxEK = TrovaMax(erroreK);

            //massimi teorici
            double maxTTq = TrovaMax(teoricoTq);
            double maxTTk = TrovaMax(teoricoTk);
            double maxTQ = TrovaMax(teoricoQ);
            double maxTK = TrovaMax(teoricoK);

            //massimi simulati
            double maxSTq = TrovaMax(simulatoTq);
            double maxSTk = TrovaMax(simulatoTk);
            double maxSQ = TrovaMax(simulatoQ);
            double maxSK = TrovaMax(simulatoK);

            double maxTq = TrovaMax(maxSTq, maxTTq);
            double maxTk = TrovaMax(maxSTk, maxTTk);
            double maxQ = TrovaMax(maxSQ, maxTQ);
            double maxK = TrovaMax(maxSK, maxTK);

            StreamWriter script = new StreamWriter(percorso + "\\script\\script.txt", false);

            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Tempo medio in coda""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\tempo_coda.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E[Tq]'");
            script.WriteLine(@"set xrange[0:1]");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxTq + 1, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\teorici" + @"\\tempoCoda.txt' title ""Teorico Tempo Coda"" with lines lt 1 lc 1, '" + percorso + "\\simulati" + @"\\tempoCoda.txt' title ""Simulato Tempo Coda"" with lines lt 2 lc 2");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Tempo medio nel sistema""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\tempo_sistema.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E[Tk]'");
            script.WriteLine(@"set xrange[0:1]");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxTk + 1, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\teorici" + @"\\tempoSistema.txt' title ""Teorico Tempo Sistema"" with lines lt 1 lc 1, '" + percorso + "\\simulati" + @"\\tempoSistema.txt' title ""Simulato Tempo Sistema"" with lines lt 2 lc 2");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Utenti medi in coda""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\utenti_coda.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E[q]'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxQ + 1, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\teorici" + @"\\utentiCoda.txt' title ""Teorico Utenti Coda"" with lines lt 1 lc 1, '" + percorso + "\\simulati" + @"\\utentiCoda.txt' title ""Simulato Utenti Coda"" with lines lt 2 lc 2");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Utenti medi nel sistema""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\utenti_sistema.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E[k]'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxK + 1, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\teorici" + @"\\utentiSistema.txt' title ""Teorico Utenti Sistema"" with lines lt 1 lc 1, '" + percorso + "\\simulati" + @"\\utentiSistema.txt' title ""Simulato Utenti Sistema"" with lines lt 2 lc 2");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Errore % Utenti medi nel sistema""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\errore_utenti_sistema.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E %'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxEK + 5, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\errore" + @"\\utentiSistema.txt' title ""Errore percentuale Utenti Sistema"" with lines lt 1 lc 1");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Errore % Utenti medi in coda""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\errore_utenti_coda.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E %'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxEQ + 5, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\errore" + @"\\utentiCoda.txt' title ""Errore percentuale Utenti Coda"" with lines lt 1 lc 1");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Errore % Tempo medio nel sistema""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\errore_tempo_sistema.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E %'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxETk + 5, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\errore" + @"\\tempoSistema.txt' title ""Errore percentuale Tempo Sistema"" with lines lt 1 lc 1");
            script.WriteLine(@"unset multiplot");
            script.WriteLine(@"set terminal jpeg");
            script.WriteLine(@"set title ""Errore % Tempo medio in coda""");
            script.WriteLine(@"set output '" + percorso + "\\grafici" + "\\errore_tempo_coda.jpeg'");
            script.WriteLine(@"set key left box");
            script.WriteLine(@"set multiplot");
            script.WriteLine(@"set xlabel 'RHO fattore di utilizzazione'");
            script.WriteLine(@"set ylabel 'E %'");
            script.WriteLine(@"set xrange[0:1]  ");
            script.WriteLine(@"set yrange[0:" + Math.Round(maxETq + 5, 0).ToString() + "]");
            script.WriteLine(@"set style data lines");
            script.WriteLine(@"plot '" + percorso + "\\errore" + @"\\tempoCoda.txt' title ""Errore percentuale Tempo Coda"" with lines lt 1 lc 1");
            script.WriteLine(@"unset multiplot");

            script.Close();
        }

        public static void StampaErrore(string percorso, List<double> erroreK, List<double> erroreQ, List<double> erroreTq, List<double> erroreTk, List<double> rho)
        {
            StreamWriter fErroretempoCoda = new StreamWriter(percorso + "\\errore\\tempoCoda.txt", false);
            StreamWriter fErroretempoSistema = new StreamWriter(percorso + "\\errore\\tempoSistema.txt", false);
            StreamWriter fErroreUtentiCoda = new StreamWriter(percorso + "\\errore\\utentiCoda.txt", false);
            StreamWriter fErroreUtentiSistema = new StreamWriter(percorso + "\\errore\\utentiSistema.txt", false);

            for (int i = 0; i < rho.Count; i++)
            {

                fErroretempoCoda.WriteLine("{0} {1}", rho[i].ToString().Replace(',', '.'), erroreTq[i].ToString().Replace(',', '.'));
                fErroretempoSistema.WriteLine("{0} {1}", rho[i].ToString().Replace(',', '.'), erroreTk[i].ToString().Replace(',', '.'));
                fErroreUtentiCoda.WriteLine("{0} {1}", rho[i].ToString().Replace(',', '.'), erroreQ[i].ToString().Replace(',', '.'));
                fErroreUtentiSistema.WriteLine("{0} {1}", rho[i].ToString().Replace(',', '.'), erroreK[i].ToString().Replace(',', '.'));
            }

            fErroretempoCoda.Close();
            fErroretempoSistema.Close();
            fErroreUtentiCoda.Close();
            fErroreUtentiSistema.Close();
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonSimula.Enabled = false;
        }

        private void textBoxUtenti_Leave(object sender, EventArgs e)
        {
            if(textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void textBoxCoda_Leave(object sender, EventArgs e)
        {
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void buttonOutput_Click(object sender, EventArgs e)
        {
            folderBrowserDialogFileOutput.ShowDialog();
            percorso = folderBrowserDialogFileOutput.SelectedPath;
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void CreaGrafici()
        {
            string Pgm = @"C:\Program Files (x86)\gnuplot\bin\gnuplot.exe";
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            string path = percorso + "\\script\\script.txt";
            gnupStWr.WriteLine("load '" + path + "'");
            gnupStWr.Flush();
            gnupStWr.WriteLine("quit");
            gnupStWr.Flush();
            extPro.Close();

        }


        private void buttonSimula_Click(object sender, EventArgs e)
        {

            //disabilito il form
            textBoxRho.Enabled = false;
            textBoxCoda.Enabled = false;
            textBoxUtenti.Enabled = false;
            buttonOutput.Enabled = false;
            buttonReset.Enabled = false;
            buttonSimula.Enabled = false;

            //imposto i parametri
            NUTENTI = Convert.ToInt32(textBoxUtenti.Text);
            Y = Convert.ToInt32(textBoxCoda.Text);
            percorso = folderBrowserDialogFileOutput.SelectedPath;
            INCR_LAMBDA = Convert.ToDouble(textBoxRho.Text.Replace('.',','));
            percorso = CreaCartelle(percorso);
            //inizio simulazione
            List<double> tempoSistemaMedio = new List<double>();
            List<double> tempoCodaMedio = new List<double>();
            List<double> utentiSistemaMedio = new List<double>();
            List<double> utentiCodaMedio = new List<double>();
            List<double> tempoServizioMedio = new List<double>();
            List<double> r = new List<double>();


            List<double> TtempoSistemaMedio = new List<double>();
            List<double> TtempoCodaMedio = new List<double>();
            List<double> TutentiSistemaMedio = new List<double>();
            List<double> TutentiCodaMedio = new List<double>();
            List<double> TtempoServizioMedio = new List<double>();
            DateTime inizio = DateTime.Now;
            double tqTeorico, tkTeorico, kTeorico, qTeorico, txTeorico;
            for (lambda = 0.01; lambda <= 1; lambda += INCR_LAMBDA)
            {
                inizializzaSimulazione();
                simulazione();
                if( NUTENTI <= Y)
                    teoricoMM1(out kTeorico, out qTeorico, out tqTeorico, out tkTeorico, out txTeorico);
                else
                    teoricoMM1Y(out kTeorico, out qTeorico, out tqTeorico, out tkTeorico, out txTeorico);
                r.Add(Math.Round(lambda / (double)MU, 2));

                TtempoCodaMedio.Add(Math.Round(tqTeorico, 15));
                TtempoSistemaMedio.Add(Math.Round(tkTeorico, 15));
                TutentiCodaMedio.Add(Math.Round(qTeorico, 15));
                TutentiSistemaMedio.Add(Math.Round(kTeorico, 15));
                TtempoServizioMedio.Add(Math.Round(txTeorico, 15));

                //if (i == 80)
                //    Console.ReadLine();

                tempoCodaMedio.Add(Math.Round(MU * accumula_tq / (double)nUtentiGenerati, 15));
                tempoSistemaMedio.Add(Math.Round(MU * (accumula_tx + accumula_tq) / (double)nUtentiGenerati, 15));
                utentiCodaMedio.Add(Math.Round(accumula_q / (double)nUtentiGenerati, 15));
                utentiSistemaMedio.Add(Math.Round(accumula_k / (double)nUtentiGenerati, 15));
                tempoServizioMedio.Add(Math.Round(accumula_tx / (double)nUtentiGenerati, 15));
                if(lambda == 0)
                    progressBarAvanzamentoSImulazione.Value = Convert.ToInt32(1);
                else
                    progressBarAvanzamentoSImulazione.Value = Convert.ToInt32(lambda * 100);
            }
            List<double> erroreK, erroreQ, erroreTk, erroreTq;
            erroreK = new List<double>();
            erroreQ = new List<double>();
            erroreTk = new List<double>();
            erroreTq = new List<double>();
            CalcolaErrore(tempoSistemaMedio, tempoCodaMedio, utentiSistemaMedio, utentiCodaMedio, TtempoSistemaMedio, TtempoCodaMedio, TutentiSistemaMedio, TutentiCodaMedio, out erroreTk, out erroreTq, out erroreK, out erroreQ);
            StampaErrore(percorso, erroreK, erroreQ, erroreTq, erroreTk, r);
            stampaRisultatiSimulati(percorso, r, tempoCodaMedio, tempoSistemaMedio, utentiSistemaMedio, utentiCodaMedio, tempoServizioMedio);
            stampaTeorici(percorso, r, TtempoCodaMedio, TtempoSistemaMedio, TutentiSistemaMedio, TutentiCodaMedio, TtempoServizioMedio);
            CreaScript(erroreTq, erroreTk, erroreK, erroreQ, TtempoCodaMedio, TtempoSistemaMedio, TutentiCodaMedio, TutentiSistemaMedio, tempoSistemaMedio, tempoCodaMedio, utentiSistemaMedio, utentiCodaMedio);
            CreaGrafici();
            DateTime fine = DateTime.Now;
            TimeSpan durataSimulazione = fine.Subtract(inizio);
            labelTempoSimulazione.Text = "Simulazione terminata! Tempo impiegato: " + durataSimulazione.ToString().Substring(0, 11);
            
            //riabilito il form
            textBoxRho.Enabled = true;
            textBoxCoda.Enabled = true;
            textBoxUtenti.Enabled = true;
            buttonOutput.Enabled = true;
            buttonReset.Enabled = true;
            buttonSimula.Enabled = false;
            folderBrowserDialogFileOutput.SelectedPath = "";
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            buttonSimula.Enabled = false;
            percorso = "";
            textBoxCoda.Text = "";
            textBoxUtenti.Text = "";
            progressBarAvanzamentoSImulazione.Value = 0;
            labelTempoSimulazione.Text = "";
            textBoxRho.Text = "";
        }

        private void textBoxRho_Leave(object sender, EventArgs e)
        {
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void textBoxUtenti_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void textBoxCoda_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }

        private void textBoxRho_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCoda.Text != "" && textBoxUtenti.Text != "" && percorso != "" && textBoxRho.Text != "")
                buttonSimula.Enabled = true;
            else
                buttonSimula.Enabled = false;
        }
    }

    public class Utente
    {
        double tempoServizio;
        double tempoNelSistema;
        double tempoInCoda;
        //tempo rimanente prima dell'esaurimento del tempo di servizio assegnato
        double tempoServizioRimanente;

        public double TempoServizio
        {
            get
            {
                return tempoServizio;
            }

            set
            {
                tempoServizio = value;
            }
        }

        public double TempoInCoda
        {
            get
            {
                return tempoInCoda;
            }

            set
            {
                tempoInCoda = value;
            }
        }

        public double TempoNelSistema
        {
            get
            {
                return tempoNelSistema;
            }

            set
            {
                tempoNelSistema = value;
            }
        }

        public double TempoServizioRimanente
        {
            get
            {
                return tempoServizioRimanente;
            }

            set
            {
                tempoServizioRimanente = value;
            }
        }
    }
}
