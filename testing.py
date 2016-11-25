import pprint

from wolfstreet import *
# for all data print dump style... from pprint import pprint
data = get_specific_data(5)
ana_mekan = Currencies()
# if you want print all data list type
for date, currencies in data.iteritems():
    # print "Tarih:", date
    ana_mekan.add(currencies)
    for current, pay in currencies.iteritems():
        print current, pay
# for sort by date : sorted(dct.items(), key=lambda p: p[0], reverse=True)

from wolfstreet import *
total_currencies = Currencies(5)

total_currencies.calculate_correlation()

