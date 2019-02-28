﻿using System;
using System.Collections.Generic;
using System.Text;
using Model.ViewModels.CodeViewModels;

namespace Model.Interfaces
{
    public interface ICodeManager
    {
        CodeViewModel GetById(int Id);
        bool Add(CodeViewModel NewCode);
        bool Update(CodeViewModel UpdatedCode);
        bool Remove(int Id);      
        Page GetPage(PageState pageState);
    }
}
