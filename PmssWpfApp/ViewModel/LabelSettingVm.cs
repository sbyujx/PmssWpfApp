using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.Render.FileSource;

namespace PmssWpfApp.ViewModel
{
    public class LabelSettingVm
    {
        public LabelSettingVm(LabelListSetting listSetting,LabelSetting setting)
        {
            this.listSetting = listSetting;
            this.labelSetting = setting;
        }
        public bool IsVisible
        {
            get
            {
                return this.labelSetting.RelatedAttributeLabelClass.IsVisible;
            }
            set
            {
                listSetting.RelatedLabelProperties.LabelClasses.Remove(labelSetting.RelatedAttributeLabelClass);
                labelSetting.RelatedAttributeLabelClass.IsVisible = value;
                listSetting.RelatedLabelProperties.LabelClasses.Add(labelSetting.RelatedAttributeLabelClass);
            }
        }
        public string LabelName
        {
            get
            {
                return this.labelSetting.Name;
            }
        }

        private LabelListSetting listSetting;
        private LabelSetting labelSetting;
    }
}
