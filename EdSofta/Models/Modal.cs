using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using EdSofta.Enums;
using EdSofta.Interfaces;

namespace EdSofta.Models
{
    internal class Modal: IModal
    {

        private readonly FrameworkElement mModal;
        private readonly string mName;
        public ModalState mState;

        public Modal(FrameworkElement modal)
        {
            mName = modal.Name;
            mModal = modal;
            mState = ModalState.Hidden;
        }


        public FrameworkElement getModalElement()
        {
            return mModal;
        }

        public bool isActive()
        {
            return mModal.Visibility == Visibility.Visible;
        }

        public ModalState getModalState()
        {
            return mState;
        }

        public void setModalState(ModalState state)
        {
            mState = state;
            mModal.Visibility =
                state == ModalState.Visible ? Visibility.Visible : Visibility.Hidden;
        }


        public string getModalName()
        {
            return mName;
        }

        public void close()
        {
            mState = ModalState.Hidden;
            mModal.Visibility = Visibility.Hidden;
        }


    }
}
