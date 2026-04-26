using Client.Interfaces;
using Client.Services;
using Shared.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features;
public sealed class FeedbackFeature : FeatureBase
{
    public IFeatureAccessService FeatureAccessService;

    public FeedbackFeature(IFeatureAccessService featureAccessService) : base(featureAccessService, FeatureKeys.SendFeedback)
    {
        this.FeatureAccessService = featureAccessService;
    }
}
