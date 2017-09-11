# -*- coding: utf-8 -*-
"""
Created on Sun Sep 10 21:53:27 2017

@author: daniel
"""

import random
import csv

#file = open("PopulateSQL.csv",'w')
KPI = []
Brand = ['Alpha','Beta','Delta','Gamma']
Month = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec']

ADealer = ['A','B','C']
BDealer = ['D','E','F']
DDealer = ['G','H','I']
GDealer = ['J','K','L']

Final = []
for i in range(1,501,3):

    BrandIdx = random.randint(1,4)
    MonthIdx = random.randint(1,12)
    
    
    Car = random.randint(95,450)
    Truck = random.randint(50,250)
    Total = Car + Truck
    
    if BrandIdx == 1:
        Dealer = ADealer
        
    elif BrandIdx == 2:
        Dealer = BDealer
    
    elif BrandIdx == 3:
        Dealer = DDealer
        
    else:
        Dealer = GDealer
    
    DealerIdx = random.randint(1,3)

    KPI = "Dealer_Sales"   
    one = [i,KPI, Brand[BrandIdx-1],"Dealer_{}".format(Dealer[DealerIdx-1]), 'Car', Month[MonthIdx-1],Car]
    two = [i+1,KPI, Brand[BrandIdx-1],"Dealer_{}".format(Dealer[DealerIdx-1]), 'Truck', Month[MonthIdx-1],Truck]
    thr = [i+2,KPI, Brand[BrandIdx-1],"Dealer_{}".format(Dealer[DealerIdx-1]), 'Total', Month[MonthIdx-1],Total]
    
    Final.append(one)
    Final.append(two)
    Final.append(thr)
   
with open("PopulateSQL.csv", "w") as f:
    writer = csv.writer(f)
    writer.writerows(Final)
       