using Ecommerce.Admin.Components.Pages.Categories;
using Ecommerce.Shared.Entities;
using Ecommerce.Shared.Entities.Clusters;
using Ecommerce.Shared.Entities.Features;
using Ecommerce.Shared.Entities.Templates;
using Ecommerce.Shared.Services.Clusters;
using Ecommerce.Shared.Services.Features;
using Ecommerce.Shared.Services.Shared;
using Ecommerce.Shared.Services.Templates;
using Microsoft.Extensions.Options;
using MudBlazor;
using Radzen.Blazor;

namespace Ecommerce.Admin.Components.Pages.Template;

public partial class AddEditTemplate
{

    #region Veriable
    private List<Feature> features = new List<Feature>();
    private IEnumerable<Cluster> clusters = new List<Cluster>();
    private TemplateCluster templateCluster = new TemplateCluster();
    private Feature objFeature = new Feature();

    private List<TemplateClusterFeature> litemplateClusterFeature = new List<TemplateClusterFeature>();
    private List<TemplateClusterFeature> templateClusterFeatures = new List<TemplateClusterFeature>();
    public RadzenDataGrid<TemplateClusterFeature> grid;

    private TemplateMaster TemplateMaster = new TemplateMaster();

    /*Tab List*/
    private List<TemplateCluster> clusterTabs = new List<TemplateCluster>();
    #endregion

    #region GetMethods
    protected override async Task OnInitializedAsync()
    {
        await LoadFeatures();
        await LoadClusters();
    }

    private async Task LoadFeatures()
    {
        var response = await FeatureService.GetFeaturesAsync();
        if (response.Success)
        {
            features = response.Data;
        }
        else
        {

        }
    }
    private async Task LoadClusters()
    {
        try
        {
            var response = await clusterService.GetClustersAsync();
            if (response.Success)
            {
                clusters = response.Data;
            }
            else
            {
                clusters = new List<Cluster>();
            }
        }
        catch (Exception ex)
        {
            clusters = new List<Cluster>();
        }
    }

    #endregion'

    #region DetailList
    private void AddToList()
    {
        if (templateCluster.Id == 0)
        {
            Snackbar.Add("Selec clusters: ", Severity.Info);
            return;
        }
        else if (objFeature.Id == 0)
        {
            Snackbar.Add("Select Feature: ", Severity.Info);
            return;
        }
        else
        {
            var fet = features.FirstOrDefault(a => a.Id == objFeature.Id);
            var feature = new TemplateClusterFeature { FeatureId = objFeature.Id, Feature = fet };
            litemplateClusterFeature.Add(feature);
            feature.Id = 0;
            StateHasChanged();
        }
    }

    private void RemoveFromList(TemplateClusterFeature feature)
    {
        litemplateClusterFeature.Remove(feature);
        StateHasChanged();  // Ensure the state is updated
    }
    #endregion



    #region Popup

    public async Task OpenPopup()
    {
        try
        {


            var parameters = new DialogParameters
        {
            { "Features", features },
            { "Clusters", clusters },
            { "TemplateClusterFeatures", templateClusterFeatures }
        };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var result = await DialogService.Show<TemplateDialog>("Template Dialog", parameters, options).Result;

            if (!result.Cancelled)
            {
                var resultData = (dynamic)result.Data;
                templateClusterFeatures = resultData.TemplateClusterFeatures;
                Cluster cluster = resultData.cluster;
                TemplateCluster obj = new TemplateCluster();

                obj.cluster = cluster;
                obj.ClusterId = cluster.Id;
                obj.Features = templateClusterFeatures;
                clusterTabs.Add(obj);
                templateClusterFeatures = new List<TemplateClusterFeature>();
                StateHasChanged();
            }

        }

        catch (Exception ex)
        {

        }
    }
    #endregion


    #region Save
    private async Task SaveTemplateMaster()
    {
        try
        {
            if (TemplateMaster != null)
            {

                TemplateMaster.Clusters = clusterTabs;
                ServiceResponse<TemplateMaster> response;
                if (TemplateMaster.Id > 0)
                {
                    response = await TemplateService.UpdateTemplateMasterAsync(TemplateMaster);
                }
                else
                {
                    response = await TemplateService.AddTemplateMasterAsync(TemplateMaster);
                }

                if (response.Success)
                {

                    Snackbar.Add("Template Master and Clusters saved successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add(response.Message, Severity.Error);
                }
            }
            else
            {
                Snackbar.Add("Template Master is null", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
        }
    }
    #endregion
}

