import pprint
from wolfstreet import *

total_currencies = Currencies(8)
total_currencies.calculate_correlation()
cl_sorted_currencies = total_currencies.cal_correlation_make_class_format()

# Save xml type for C# program
convert_xml_format(cl_sorted_currencies)

cl_sorted_currencies = sort_and_add_indis(cl_sorted_currencies)
for currentss in cl_sorted_currencies:
    print currentss.order, "\t", \
        currentss.name, "\t", "%.5f" % currentss.corelation, "\t", \
        map(lambda x: "%.4f" % x, currentss.all_data)

start_money = 1000
mutation_possibility = 0.005
# one_generation = Generation(money=start_money)
# one_generation.create_fx_values(cl_sorted_currencies)
# one_generation.population.sort(key=lambda x: x.fi, reverse=False)
# elite_person = one_generation.population[0]
# one_generation.population = assign_fitness_values(one_generation.population,start_money)
#
# after_generation = roulette(one_generation.population, elite_person, start_money)
# after_generation = cross(after_generation)
# after_generation = mutation(after_generation, mutation_possibility)
# best = find_best_person(after_generation, start_money)
# print "\n\n Ikinci jenerasyon:"
# for icerik in after_generation.population:
#     print icerik.fi
# print "En iyi degerin fi: ", best.fi
best_person = genetic_loop(start_money, cl_sorted_currencies, mutation_possibility, 10)
print "Best Chromozome values:"
print "Chromozome Gens: %s \nChromozome fi value: %s \nChromozome distance: %s \n " % (
    best_person.gens,
    best_person.fi,
    best_person.fi_distance
)
