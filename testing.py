import pprint
from wolfstreet import *

total_currencies = Currencies(8)
total_currencies.calculate_correlation()
CL_sorted_currencies = total_currencies.cal_correlation_make_class_format()
# Print all class content
# for currentss in CL_currencies:
#    print currentss.name,currentss.corelation,currentss.all_data

# Save xml type for C# program
convert_xml_format(CL_sorted_currencies)

CL_sorted_currencies = sort_and_add_indis(CL_sorted_currencies)
for currentss in CL_sorted_currencies:
    print currentss.order, "\t", \
        currentss.name, "\t", "%.5f" % currentss.corelation, "\t", \
        map(lambda x: "%.4f" % x, currentss.all_data)

one_generation = Generation(money=1000)
bireyler = one_generation.get_population()
one_generation.create_fx_values(CL_sorted_currencies)
one_generation.population.sort(key=lambda x: x.fi, reverse=True)
elite_person = one_generation.population[0]
assign_fitness_values(one_generation.population)
roulette(one_generation.population)
# bu sekilde guncel kur bilgisini kullanmak icin cekeceksin
# print "%.4f" % currentss.all_data.pop()

