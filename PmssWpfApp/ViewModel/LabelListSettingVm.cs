using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.Render.FileSource;

namespace PmssWpfApp.ViewModel
{
    public class LabelListSettingVm
    {
        public LabelListSettingVm(LabelListSetting listSetting)
        {
            foreach (var setting in listSetting.LabelSettings)
            {
                //var vm = new LabelSettingVm(listSetting.RelatedLabelProperties.LabelClasses, setting.RelatedAttributeLabelClass, setting.Name);
                var vm = new LabelSettingVm(listSetting, setting);
                Labels.Add(vm);
            }
        }
        public List<LabelSettingVm> Labels { get; set; } = new List<LabelSettingVm>();
    }
}
