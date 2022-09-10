using Microsoft.AspNetCore.Components;
using WealthBuilder.Models;
using WealthBuilder.Services;

namespace WealthBuilder.Pages
{
    public partial class Index
    {
        [Inject]
        public InvestorProfile InvestorProfile { get; set; }

        [Inject]
        public WealthForecastService ForecastService { get; set; }

        private List<LifeStyle> RetirementLifeStyleList { get; set; }

        //Confirmation message related
        private bool DisplayConfirmationMessage { get; set; }
        private string ConfirmationMessage { get; set; }

        //Validation related
        private Confirmation WarningDialog { get; set; }
        private List<string> InputErrorFieldNames { get; set; }

        
        protected override async Task OnInitializedAsync()
        {
            DisplayConfirmationMessage = false;
            RetirementLifeStyleList = FinanceUtils.GetRetirementLifestyleList();

            SetupDefaultInvestorProfile();
        }

        private void OnLifestyleChange(ChangeEventArgs e)
        {
            int lifeStyleId = 0;
            if (int.TryParse(e.Value as string, out lifeStyleId))
            {                
                InvestorProfile.RetirementLifeStyle = RetirementLifeStyleList.FirstOrDefault(x => x.Id == lifeStyleId);
                AdjustSavingsAndDistributionAmounts(InvestorProfile.RetirementLifeStyle);
            }
        }

        private void SetupDefaultInvestorProfile()
        {            
            const int DefaultStartingAge = 22;
            const int DefaultNumberOfWorkingYears = 30;
            const int DefaultLifestyleLevel = 3;    //Upper-Middle Class

            if (InvestorProfile.WorkStartAge == 0)
            {
                InvestorProfile.WorkStartAge = DefaultStartingAge;
            }

            if (InvestorProfile.NumberOfWorkingYears == 0)
            {
                InvestorProfile.NumberOfWorkingYears = DefaultNumberOfWorkingYears;
            }

            if (InvestorProfile.RetirementLifeStyle == null ||
                InvestorProfile.RetirementLifeStyle.Id == 0)
            {
                InvestorProfile.RetirementLifeStyle = RetirementLifeStyleList.FirstOrDefault(x => x.Id == DefaultLifestyleLevel);
            }

            AdjustSavingsAndDistributionAmounts(InvestorProfile.RetirementLifeStyle);
        }

        private void AdjustSavingsAndDistributionAmounts(LifeStyle lifeStyle)
        {
            if (InvestorProfile.RetirementLifeStyle == null)
            {
                return;
            }

            //if (InvestorProfile.AnnualTaxableSavingAmountPV == 0)
            {
                InvestorProfile.AnnualTaxableSavingAmountPV = lifeStyle.AnnualTaxableSavingAmount;
            }

            //if (InvestorProfile.AnnualRetirementSavingAmountPV == 0)
            {
                InvestorProfile.AnnualRetirementSavingAmountPV = lifeStyle.AnnualRetirementSavingAmount;
            }

            //if (InvestorProfile.AnnualWithdrawalAmountPV == 0)
            {
                InvestorProfile.AnnualWithdrawalAmountPV = lifeStyle.AnnualRetirementWithdrawalAmount;
            }

        }

        private bool ValidateInputValues()
        {
            const int MaxAnnualRetirementAmount = 20500;

            InputErrorFieldNames = new List<string>();

            if (InvestorProfile.WorkStartAge == 0)
            {
                InputErrorFieldNames.Add("Your current age");
            }

            if (InvestorProfile.NumberOfWorkingYears == 0)
            {
                InputErrorFieldNames.Add("Your number of working years");
            }

            if (InvestorProfile.AnnualRetirementSavingAmountPV == 0)
            {
                InputErrorFieldNames.Add("Your annual retirement saving amount");
            }
            else if (InvestorProfile.AnnualRetirementSavingAmountPV > MaxAnnualRetirementAmount * 2)  //Allow for the max of a couple
            {
                InputErrorFieldNames.Add("Your annual retirement saving amount may not exceed $41,000.");
            }

            if (InvestorProfile.AnnualTaxableSavingAmountPV == 0)
            {
                InputErrorFieldNames.Add("Your annual taxable saving amount");
            }

            if (InvestorProfile.AnnualWithdrawalAmountPV == 0)
            {
                InputErrorFieldNames.Add("Your annual withdrawal amount");
            }

            return InputErrorFieldNames.Count == 0;
        }

        private void ConfirmAmount()
        {
            const int AnnualSavingAmountGoodThreshold = 30000;

            if (ValidateInputValues() == false)
            {
                ShowWarningMessage();
                return;
            }


            //To-do: Add more funny messages
            //But just so you know. Only a red-neck would save only x a year and think that's a upper-middle class lifestyle. 
            if (InvestorProfile.AnnualTaxableSavingAmountPV >= AnnualSavingAmountGoodThreshold)
            {
                ConfirmationMessage = $"Great! Saving {InvestorProfile.AnnualTaxableSavingAmountPV.ToString("C0")} a year will put you in an excellent place when you retire.";
            }
            else
            {
                ConfirmationMessage = $"Great! You have chosen to save {InvestorProfile.AnnualTaxableSavingAmountPV.ToString("C0")} a year. When you make more later, remember to increase your saving as well.";
            }

            DisplayConfirmationMessage = true;
        }

        #region WarningDialog
        async Task OnConfirm()
        {
            WarningDialog.Hide();
        }

        void OnCancel()
        {
            WarningDialog.Hide();
        }

        private void ShowWarningMessage()
        {
            WarningDialog.Show();
        }
        #endregion


    }
}
