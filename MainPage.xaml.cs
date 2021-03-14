using RouleteIRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Roulete_IRS
{
    public partial class MainPage : ContentPage
    {
        private Session session;

        public MainPage()
        {
            InitializeComponent();
            SessionInfoGrid.IsVisible = false;
            NextBetLabel.IsVisible = false;
            DozenGrid.IsVisible = false;
            SubmitBtn.IsEnabled = false;

            session = new Session();
            session.BettingUnits.Add(1);
            session.CancellationWay = 2;
            session.LastUnits = 1;
        }

        void StartingUnitEntered(object sender, EventArgs e)
        {
            decimal unitAmount;

            if (decimal.TryParse(StartingUnitTextbox.Text, out unitAmount) &&
                decimal.Parse(StartingUnitTextbox.Text) > 0m &&
                NumberPicker.SelectedItem != null &&
                ResultPicker.SelectedItem != null)
                SubmitBtn.IsEnabled = true;
            else
                SubmitBtn.IsEnabled = false;
        }

        void NumberSelected(object sender, EventArgs e)
        {
            decimal unitAmount;

            if (NumberPicker.SelectedItem == null)
                SubmitBtn.IsEnabled = false;
            else if (decimal.TryParse(StartingUnitTextbox.Text, out unitAmount) &&
                     decimal.Parse(StartingUnitTextbox.Text) > 0m &&
                     ResultPicker.SelectedItem != null)
                SubmitBtn.IsEnabled = true;
        }

        void ResultSelected(object sender, EventArgs e)
        {
            decimal unitAmount;

            if (ResultPicker.SelectedItem == null)
                SubmitBtn.IsEnabled = false;
            else if (decimal.TryParse(StartingUnitTextbox.Text, out unitAmount) &&
                     decimal.Parse(StartingUnitTextbox.Text) > 0m &&
                     NumberPicker.SelectedItem != null)
                SubmitBtn.IsEnabled = true;
        }

        void SubmitBtnClicked(object sender, EventArgs e)
        {
            StartingUnitTextbox.IsEnabled = false;

            session.SpinNumber++;
            SessionInfoGrid.IsVisible = true;

            if (session.UnitAmount == 0.00m)
                session.UnitAmount = decimal.Parse(StartingUnitTextbox.Text);

            if (session.BettingUnits.Count() < 2)
                session.BetPerDozen = session.UnitAmount;

            int dozen = session.GetDozenNumber(int.Parse(NumberPicker.SelectedItem.ToString()));

            if (dozen > 0)
                session.Dozens.Add(dozen);

            if (ResultPicker.SelectedItem.ToString() == "W")
            {
                session.NumberOfConsecutiveLosses = 0;
                session.Bankroll = session.Bankroll + (session.UnitAmount * session.LastUnits);

                int unitsWon = session.LastUnits;

                while (unitsWon > 0 && session.BettingUnits.Count() > 0)
                {
                    session.BettingUnits[0] = session.BettingUnits.ElementAt(0) - 1;
                    unitsWon--;

                    if (session.BettingUnits.ElementAt(0) == 0)
                        session.BettingUnits.RemoveAt(0);
                }
            }
            else
            {
                session.NumberOfConsecutiveLosses++;
                session.Bankroll -= ((session.UnitAmount * 2) * session.LastUnits);

                if (session.NumberOfConsecutiveLosses >= session.CancellationWay)
                    session.CancellationWay = session.NumberOfConsecutiveLosses;

                if (session.BettingUnits.Count() == 1)
                {
                    session.BettingUnits.Add(session.BettingUnits.ElementAt(0));
                    session.BettingUnits.Add(session.BettingUnits.ElementAt(0));
                }
                else
                {
                    session.BettingUnits.Add(session.LastUnits);
                    session.BettingUnits.Add(session.LastUnits);
                }
            }

            session.LastResult = ResultPicker.SelectedItem.ToString();

            if (session.BettingUnits.Count() >= session.CancellationWay &&
                session.Bankroll <= 0m)
            {
                int units = getUnits();
                session.BetPerDozen = session.UnitAmount * units;
                session.LastUnits = units;
            }
            else
            {
                session.SpinNumber = 0;
                session.Bankroll = 0m;
                session.BettingUnits.Add(1);
                session.CancellationWay = 2;
                session.LastUnits = 1;
                session.BetPerDozen = session.UnitAmount;
            }

            SpinNumberLabel.Text = session.SpinNumber.ToString();
            BankrollLabel.Text = session.Bankroll.ToString("C");

            int dozen1 = session.Dozens.Last();
            int dozen2 = 0;

            for (int i = session.Dozens.Count - 1; i >= 0; i--)
            {
                if (session.Dozens.ElementAt(i) != dozen1)
                {
                    dozen2 = session.Dozens.ElementAt(i);
                    break;
                }
            }

            Dozen1Amount.Text = "";
            Dozen2Amount.Text = "";
            Dozen3Amount.Text = "";

            if (dozen1 == 1 || dozen2 == 1)
                Dozen1Amount.Text = session.BetPerDozen.ToString("C");
            if (dozen1 == 2 || dozen2 == 2)
                Dozen2Amount.Text = session.BetPerDozen.ToString("C");
            if (dozen1 == 3 || dozen2 == 3)
                Dozen3Amount.Text = session.BetPerDozen.ToString("C");

            if (session.Dozens.Count() >= 2 && dozen1 > 0 && dozen2 > 0)
            {
                NextBetLabel.IsVisible = true;
                DozenGrid.IsVisible = true;
            }
            else
            {
                NextBetLabel.IsVisible = false;
                DozenGrid.IsVisible = false;
            }
        }

        private int getUnits()
        {
            if (session.LastResult == "W")
            {
                int units = 0;

                for (int i = 0; i < session.CancellationWay; i++)
                {
                    units = units + session.BettingUnits.ElementAt(i);
                }

                return units;
            }

            return 1;
        }
    }
}
