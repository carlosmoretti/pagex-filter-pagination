# 😀 O que é o Pagex?
Nada mais, nada menos, que uma biblioteca BRASILEIRA de paginação/filtro/ordenação de dados para uma grid.

# Como usar?
Levando em consideração que o código fonte é aberto, você pode clonar e usar o fonte propriamente dito, ou você pode baixar o pacote diretamente do nuget (juro que vou tentar manter ele lá atualizado).
https://www.nuget.org/packages/pagex.filter.pagination/

### Implementando
Criei um projeto teste usando a ferramenta pra vocês visualizarem como funciona.
https://github.com/carlosmoretti/pagex-filter-pagination-example

### Classe de Filtro
Inicialmente, você precisa ter uma classe específica para receber o seu filtro. Não recomendo que utilize o ViewModel nesse ponto e sim crie uma classe para receber os filtros.

```csharp
public class UsuarioFilter
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
```

O Pagex tem um recurso que faz toda a automatização necessária para paginar, filtrar, etc. Para isso, herde a classe PageFilter, passando um ViewModel e a própria classe de Filtro.

```csharp
public class UsuarioFilter : PageFilter<UsuarioViewModel, UsuarioFilter>
{
    public int? Id { get; set; }
    public string Name { get; set; }
}
```

Para fazer o filtro de dados, o sistema te obriga a sobreescrever o método Filter, para isso, faça.
```csharp
public class UsuarioFilter : PageFilter<UsuarioViewModel, UsuarioFilter>
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public override IEnumerable<UsuarioViewModel> Filter(IEnumerable<UsuarioViewModel> value)
        {
            if (Id != null)
                value = value.Where(d => d.Id == Id.Value);

            if (!String.IsNullOrEmpty(Name))
                value = value.Where(d => d.Name.ToUpper().Contains(Name.ToUpper()));

            return value;
        }
    }
```

It's done!

### Controller
Nosso Controller não terá muita novidade. De primeira, o Pagex só funciona em requisições POST, visto que você não pode receber tipos complexos de requisições GET.

```csharp
[HttpPost]
public FilterResponse<UsuarioViewModel, UsuarioFilter> Get(UsuarioFilter page)
{
    List<Usuario> str = new List<Usuario>();
    for (int i = 0; i < 305; i++)
        str.Add(new Usuario
        {
            Id = i,
            Name = $"Carlos-{i}"
        });

    var res = _mapper.Map<IEnumerable<Usuario>, IEnumerable<UsuarioViewModel>>(str);
    return page.Paginate(res);
}
```

Algumas observações:
* Nosso Controller recebe a classe UsuarioFilter criada anteriormente e retorna um FilterResponse.
* FilterResponse<T, D> onde T : ViewModel, tipo que os dados serão retornados, D : Filtro.
* O próprio atributo que você receberá fornece o método Paginate, que também filtra os dados caso os atributos de filtro sejam populados.

### Documentação
A requisição POST receberá:

```json
{
  "id": 0,
  "name": "string",
  "page": 0,
  "itensPerPage": 0,
  "orderColumn": "string",
  "orderDirection": "string"
}
```
Onde:
* Id, Name = Atributos da classe derivada UsuarioFilter. **Caso estes atributos sejam populados, a classe base FilterPage chamará a sobreescrita do método Filter, da sua classe derivada.**
* page, itensPerPage, orderColumn e orderDirection = Atributos da classe base FilterPage.

## Cenários
### Cenário 01 (Request e Response)
```json
{
  "page": 1,
  "itensPerPage": 10,
  "orderColumn": "Id",
  "orderDirection": "ASC"
}
```
```json
{
  "response": [
    {
      "name": "Carlos-0",
      "id": 0
    },
    {
      "name": "Carlos-1",
      "id": 1
    },
    {
      "name": "Carlos-2",
      "id": 2
    },
    {
      "name": "Carlos-3",
      "id": 3
    },
    {
      "name": "Carlos-4",
      "id": 4
    },
    {
      "name": "Carlos-5",
      "id": 5
    },
    {
      "name": "Carlos-6",
      "id": 6
    },
    {
      "name": "Carlos-7",
      "id": 7
    },
    {
      "name": "Carlos-8",
      "id": 8
    },
    {
      "name": "Carlos-9",
      "id": 9
    }
  ],
  "filter": null,
  "page": 1,
  "itensPerPage": 10,
  "pageCount": 30,
  "filteredCount": 305,
  "requestCount": 10,
  "totalRowsCount": 305,
  "orderColumn": "Id",
  "orderDirection": "ASC",
  "pages": [
    {
      "label": "1",
      "pageNumber": 1,
      "active": true
    },
    {
      "label": "2",
      "pageNumber": 2,
      "active": false
    },
    {
      "label": "3",
      "pageNumber": 3,
      "active": false
    },
    {
      "label": "4",
      "pageNumber": 4,
      "active": false
    },
    {
      "label": "5",
      "pageNumber": 5,
      "active": false
    },
    {
      "label": "6",
      "pageNumber": 6,
      "active": false
    },
    {
      "label": "7",
      "pageNumber": 7,
      "active": false
    },
    {
      "label": "8",
      "pageNumber": 8,
      "active": false
    },
    {
      "label": "9",
      "pageNumber": 9,
      "active": false
    },
    {
      "label": "10",
      "pageNumber": 10,
      "active": false
    },
    {
      "label": ">",
      "pageNumber": 2,
      "active": false
    },
    {
      "label": ">>",
      "pageNumber": 31,
      "active": false
    }
  ]
}
```

### Cenário 02 (Request e Response com Filtro - Nomes onde há o número 2)
```json
{
  "name": "2",
  "page": 14,
  "itensPerPage": 10,
  "orderColumn": "Id",
  "orderDirection": "ASC"
}
```

```json
{
  "response": [
    {
      "name": "Carlos-292",
      "id": 292
    },
    {
      "name": "Carlos-293",
      "id": 293
    },
    {
      "name": "Carlos-294",
      "id": 294
    },
    {
      "name": "Carlos-295",
      "id": 295
    },
    {
      "name": "Carlos-296",
      "id": 296
    },
    {
      "name": "Carlos-297",
      "id": 297
    },
    {
      "name": "Carlos-298",
      "id": 298
    },
    {
      "name": "Carlos-299",
      "id": 299
    },
    {
      "name": "Carlos-302",
      "id": 302
    }
  ],
  "filter": null,
  "page": 14,
  "itensPerPage": 10,
  "pageCount": 13,
  "filteredCount": 139,
  "requestCount": 9,
  "totalRowsCount": 305,
  "orderColumn": "Id",
  "orderDirection": "ASC",
  "pages": [
    {
      "label": "<<",
      "pageNumber": 5,
      "active": false
    },
    {
      "label": "<",
      "pageNumber": 13,
      "active": false
    },
    {
      "label": "5",
      "pageNumber": 5,
      "active": false
    },
    {
      "label": "6",
      "pageNumber": 6,
      "active": false
    },
    {
      "label": "7",
      "pageNumber": 7,
      "active": false
    },
    {
      "label": "8",
      "pageNumber": 8,
      "active": false
    },
    {
      "label": "9",
      "pageNumber": 9,
      "active": false
    },
    {
      "label": "10",
      "pageNumber": 10,
      "active": false
    },
    {
      "label": "11",
      "pageNumber": 11,
      "active": false
    },
    {
      "label": "12",
      "pageNumber": 12,
      "active": false
    },
    {
      "label": "13",
      "pageNumber": 13,
      "active": false
    },
    {
      "label": "14",
      "pageNumber": 14,
      "active": true
    }
  ]
}
```

### Cenário 03 (Request e Response com Filtro - Nomes onde há o número 2 -- ORDEM DESC)
```json
{
  "name": "22",
  "page": 2,
  "itensPerPage": 10,
  "orderColumn": "Id",
  "orderDirection": "DESC"
}
```

```json
{
  "response": [
    {
      "name": "Carlos-122",
      "id": 122
    },
    {
      "name": "Carlos-22",
      "id": 22
    }
  ],
  "filter": null,
  "page": 2,
  "itensPerPage": 10,
  "pageCount": 1,
  "filteredCount": 12,
  "requestCount": 2,
  "totalRowsCount": 305,
  "orderColumn": "Id",
  "orderDirection": "DESC",
  "pages": [
    {
      "label": "<<",
      "pageNumber": 1,
      "active": false
    },
    {
      "label": "<",
      "pageNumber": 1,
      "active": false
    },
    {
      "label": "1",
      "pageNumber": 1,
      "active": false
    },
    {
      "label": "2",
      "pageNumber": 2,
      "active": true
    }
  ]
}
```

### Implementação com Angular
```typescript
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent implements OnInit {

  public itens$: Observable<any>;
  
  public actualPage: number;
  public itensPerPage: number;
  public orderColumn: string;
  public orderDirection: string;
  
  public name: string;
  public id: number;

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    this.name = null;
    this.id = null;
    this.actualPage = 1;
    this.itensPerPage = 10;
    this.orderColumn = "Id";
    this.orderDirection = "ASC";
    this.updateTable();
  }

  updateTable() {
    console.log(this.id);
    this.itens$ = this.http.post("http://localhost:64588/api/Teste", {
      "name": this.name,
      "id": this.id,
      "page": this.actualPage,
      "itensPerPage": this.itensPerPage,
      "orderColumn": this.orderColumn,
      "orderDirection": this.orderDirection
    });
  }

  updatePage(page: number) {
    this.actualPage = page;
    this.updateTable();
  }

  changeItensPerPage(obj: HTMLSelectElement) {
    this.actualPage = 1;
    this.itensPerPage = parseInt(obj.value);
    this.updateTable();
  }

  filterTable(id: number, name: string) {
    this.id = id
    this.name = name
    this.actualPage = 1
    this.updateTable();
  }

}
```
