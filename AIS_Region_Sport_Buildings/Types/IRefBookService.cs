using System;
using System.Collections.Generic;

namespace AIS_Region_Sport_Buildings.Types
{
    interface IRefBookService<T>
    {
        // получение записей
        List<T> GetAll();


        // добавление новой записи
        bool Create(string title);


        // обновление существующей записи
        bool Update(int id, string newTitle);


        // удаление выбранной записи
        bool Delete(int id);
        
    }
}

