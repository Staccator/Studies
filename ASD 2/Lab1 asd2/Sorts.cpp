#include <iostream>
#include <unistd.h>
#include <iomanip>
using namespace std;

struct node{
    int val;
    node* next;
public:
    node(int v,node* n):val(v),next(n){};
};
struct list{
    node* first;
    list* next;
public:
    list(node* n,list* l):first(n),next(l){}
};

void printlist(node* head);

void swap(int&a,int&b){int tmp=a; a=b;b=tmp;}

void print_tab(int tab[],int n){

    for(int i = 0;i<n;i++)
    cout<<setw(2)<<tab[i]<<" ";
    cout<<endl;
}

void selectionsort(int * tab, int n){
    int min;
    for(int i = 0;i<n-1;i++)
    {
        min = i;
        for (int j = i+1; j < n; j++)
        {
            if (tab[j] < tab[min]) min = j;
        }
        swap(tab[i],tab[min]);
    }
}

void bubblesort(int *tab,int n)
{
    for(int i=0;i<n-1;i++)
        for(int j = 0;j<n-1-i;j++)
            if(tab[j]>tab[j+1]) swap(tab[j],tab[j+1]);
}

void insertionsort(int *tab,int n){
    for(int i=1;i<n;i++)
    {
        int j=i;
        while(tab[j-1]>tab[j]) {swap(tab[j-1],tab[j]);j--; if(j==0) break;}
    }
}

int partition(int* &tab,int l, int r){
    int i=l,j=r;
    int pivot = tab[(l+r)/2];
    while(i<=j){
        while(tab[i] < pivot)
            i++;
        while(pivot < tab[j])
            j--;
        if(i<=j){
            int pom = tab[i];
            tab[i] = tab[j];
            tab[j] = pom;
            i++;
            j--;
        }
    }
    return i;
}

void quicksort(int * tab,int l,int r){
    if(!(l<r)) return;
    int m = partition(tab,l,r);
    quicksort(tab,l,m-1);
    quicksort(tab,m,r);
}

void countsort(int *tab,int n){
    int cups[50];
    int tmp[20];
    for(int i=0;i<50;i++) cups[i] = 0;
    for(int i=0;i<n;i++) cups[tab[i]]++;
    for(int i=1;i<50;i++) cups[i]+=cups[i-1];
    for(int i=n-1;i>=0;i--)tmp[--cups[tab[i]] ] = tab[i];
    for(int i=0;i<n;i++) tab[i] = tmp[i];
}

void addtoend(node* &head,node* &tail,node* &v){
    if(head == nullptr){head = tail = v;}
    else tail = tail->next = v;
}

void bucketsort(node* &head)
{
    node* heads[50];
    node* tails[50];
    for(int i=0;i<50;i++) {heads[i] =nullptr; tails[i] = nullptr;}
    node* p = head;
    node*q;
    while(p!=nullptr){
        q = p;
        p = p->next;
        q->next = nullptr;
        addtoend(heads[q->val],tails[q->val],q);
    }

    bool flag = true;
    int j;
    for(int i =0;i<49;i++){
        if(flag) if(heads[i]!=nullptr) {head = heads[i]; flag = false;j=i;continue;}
        if(heads[i]!=nullptr) {tails[j]->next = heads[i];j=i;}
    }

}

void printlist(node* head){
    node*p = head;
    while(p!=nullptr){
        cout<<p->val<<" ";
        p = p->next;
    }
    cout<<endl;
}

void merge(int * &tab, int l,int m,int r){
    int i=l;
    int j=m+1;
    int tmp[r-l+1];int k=0;
    while(i!=m+1 && j!=r+1){
        if(tab[i]<tab[j])
            tmp[k++] = tab[i++];
        else tmp[k++] = tab[j++];
    }
    if(i!=m+1){for(int l=i;l<m+1;l++) tmp[k++] = tab[i++]; }
        else {for(int l=j;l<r+1;l++) tmp[k++] = tab[j++]; }

    for(int i=0;i<r-l+1;i++){
        tab[l+i] = tmp[i];
    }
}
void mergesort(int * tab,int l,int r){
    if(!(l<r)) return;
    int m = (l+r)/2;
    mergesort(tab,l,m);
    mergesort(tab,m+1,r);
    merge(tab,l,m,r);
}

list* CreateLists(node* &head){
    list* lastlist = new list(head,nullptr);
    lastlist->next = lastlist;
    node* lastnode = head;
    node* p = head;
    node* q;
    p = p->next;
    lastnode->next = nullptr;
    while(p!=nullptr){
        q = p;
        p = p->next;
        q->next = nullptr;
        if(lastnode->val <= q->val){
            lastnode->next = q;
            lastnode = q;
        }
        else{
            lastlist->next = new list(q,lastlist->next);
            lastlist = lastlist->next;
            lastnode = q;
        }
    }
    return lastlist;
}

void Mergenodes(node* &head1,node* &head2){
    node* p1 = head1;
    node* p2 = head2;
    node *head = nullptr;node *tail = nullptr;
    node *q;
    while(p1 != nullptr && p2 != nullptr){
        if(p1->val< p2->val)
            {
             q = p1; p1 = p1->next; q->next = nullptr;
             addtoend(head,tail,q);
            }
        else
            {
            q = p2; p2 = p2->next; q->next = nullptr;
            addtoend(head,tail,q);
            }
    }
    if(p1!=nullptr){
        addtoend(head,tail,p1);
    }
    else{
        addtoend(head,tail,p2);
    }
    head1 = head;
}

node* MergeLists(list* &lista){
    while(lista->next != lista){
        list* bad = lista->next;
        Mergenodes(lista->first,lista->next->first);
        lista->next = lista->next->next;
        //printlist(lista->first);
        lista = lista->next;
        free(bad);
    }
    return lista->first;
}

void MergeSortLists(node* &head){
    list* lista = CreateLists(head);
    head = MergeLists(lista);
}

void downheap(int* tab,int n,int i){
    while(2*i<=n){
        if(2*i+1 <= n) {if (tab[2*i+1] > tab[2*i]) if (tab[2*i+1] > tab[i]) {swap(tab[i],tab[2*i+1]); i = 2*i+1; continue;}}
        if(tab[2*i] > tab[i] ) {swap(tab[i],tab[2*i]);  i = 2*i; continue;}
        return;
    }
}

void heapsort(int* tab, int n){
    for(int i= n/2;i>=1;i--) downheap(tab, n,i);
    int size = n;
    for(int i = 0;i<n-1;i++)
    {
        swap(tab[1],tab[size]);
        downheap(tab,--size,1);
    }
}

void mergetabsort(int* tab, int n){
    int lengths[n]; for(int i = 0; i < n ; i++) lengths[i] = 0;
    lengths [0]=1;
    int j=0; int NumOfSer = 1;
    for(int i = 1; i < n ; i++)
    {
        if (tab[i] >= tab[i-1])
        {
            lengths[j]++;
        }
        else
            {
            j++;lengths[j] = 1; NumOfSer++;
            }
    }
    int pos=0;
    while(NumOfSer !=1 ){
        pos = 0;
        for(int i =0;i<NumOfSer-1;i+=2){
            merge(tab,pos,pos+lengths[i]-1,pos + lengths[i] + lengths[i+1] -1);
            pos += lengths[i] + lengths[i+1];
            lengths[i/2] = lengths[i] + lengths[i+1];
        }
        if(NumOfSer % 2 == 1){
            lengths[NumOfSer/2] = lengths[NumOfSer-1];
            NumOfSer = NumOfSer/2 +1;
        }
        else NumOfSer/=2;
    }

}

int main() {
    srand(time(NULL));
    std::cout << "Hello, World!" << std::endl;
    int tab [20];
    for(int i = 0; i < 20; i++) tab[i] = rand()%20;
    print_tab(tab,20);
    //quicksort(tab,0,19);
    //countsort(tab,20);
    //mergesort(tab,0,19);
    heapsort(tab,19);
    //mergetabsort(tab,20);
    print_tab(tab,20);

    node* head=nullptr;
    for(int i=0;i<20;i++) head=new node(rand()%50,head);
    printlist(head);
    MergeSortLists(head);
    //bucketsort(head);
    printlist(head);
    return 0;
}