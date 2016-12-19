import pprint
from wolfstreet import *

total_currencies = Currencies(8)
total_currencies.calculate_correlation()
CL_currencies = total_currencies.cal_correlation_make_class_format()
# Print all class content
# for currentss in CL_currencies:
#    print currentss.name,currentss.corelation,currentss.all_data

# Save xml type
convert_xml_format(CL_currencies)

CL_currencies = sort_and_add_indis(CL_currencies)
for currentss in CL_currencies:
    print currentss.order, "\t", \
        currentss.name, "\t", "%.5f" % currentss.corelation, "\t", \
        map(lambda x: "%.4f" % x, currentss.all_data)
    #bu sekilde guncel kur bilgisini kullanmak icin cekeceksin
    print "%.4f" % currentss.all_data.pop()