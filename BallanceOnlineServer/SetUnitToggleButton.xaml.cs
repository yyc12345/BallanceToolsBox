using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BallanceOnlineServer {
    /// <summary>
    /// SetUnitToggleButton.xaml 的交互逻辑
    /// </summary>
    public partial class SetUnitToggleButton : UserControl {
        public SetUnitToggleButton() {
            InitializeComponent();
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get { return uiText.Text; } set { uiText.Text = value; } }

        /// <summary>
        /// 当前选定状态
        /// </summary>
        public bool? IsChecked {
            get { return uiToggleButton.IsChecked; }
            set { uiToggleButton.IsChecked = value; }
        }
    }
}
