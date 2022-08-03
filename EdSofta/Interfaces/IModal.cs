using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EdSofta.Enums;

namespace EdSofta.Interfaces
{
     internal interface IModal
    {
        ModalState getModalState();
        FrameworkElement getModalElement();
        void setModalState(ModalState state);
        void close();
        bool isActive();
        string getModalName();
    }
}
