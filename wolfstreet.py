from __future__ import division
import json
import random
import urllib
import xml.etree.cElementTree as ET
from datetime import *
from math import pow
from pprint import pprint
import copy


# holds information about each currency
class Currencies:
    def __init__(self, space):
        self.currencies_all_data = get_specific_data(space)
        self.currencies = {}
        self.curr_and_corel = {}

    def calculate_correlation(self):
        for keys, values in self.currencies_all_data.iteritems():
            for key, value in values.iteritems():
                if key in self.currencies.keys():
                    self.currencies[key].append(value)
                else:
                    self.currencies[key] = []
                    self.currencies[key].append(value)
                    # Check data
                    # pprint(self.currencies)

    def cal_correlation_make_class_format(self):
        currencies = []
        for key in self.currencies.iteritems():
            self.curr_and_corel[calculate_correlation_in_array(key[1])] = key
        for key, value in self.curr_and_corel.iteritems():
            # print "bu ilk key",key,"bu ilk value",value[0],value[1]
            current_name = value[0]
            currents = value[1]
            tmp_currency = Currencie(current_name, currents, key)
            currencies.append(tmp_currency)
        return currencies

    def get_currency_info(self, current_name):
        return self.curr_and_corel.get(current_name)


class Currencie:
    def __init__(self, name, all_data, corelation):
        self.name = name
        self.all_data = all_data
        self.corelation = corelation
        self.order = None


# Get currency data from API
def get_json_data(url):
    response = urllib.urlopen(url)
    data = json.loads(response.read())
    return data


# Default currencies base type 'Euro', this func convert 'TL'
def convert_currencies_tl(data):
    currency = data['rates']
    euro = data['rates']['TRY']
    for name, price in currency.iteritems():
        currency[name] = euro / price
    currency['EUR'] = euro
    del currency['TRY']
    return currency


# that attracts the exchange rates of the desired date
def get_specific_data(interval):
    total_date = {}
    for day in xrange(interval):
        total_date[str(datetime.now().date() - timedelta(day))] = convert_currencies_tl(
            get_json_data('http://api.fixer.io/latest' + str(datetime.now().date() - timedelta(day)))
        )
    return total_date


# calculates the correlation coefficient of each data set
def calculate_correlation_in_array(currencies):
    x_ort = 0
    y_ort = 0
    for count, currency in enumerate(currencies):
        x_ort += count
        y_ort += currency
    x_ort /= len(currencies)
    y_ort /= len(currencies)

    pay_toplam = 0
    payda_toplam = 0

    for count, currency in enumerate(currencies):
        pay_toplam += (count - x_ort) * (currency - y_ort)
        payda_toplam += pow(count - x_ort, 2)

    return pay_toplam / payda_toplam


# C# use "," operator for float percenteage part. Convert
def convert_xml_format(cl_currencies):
    root = ET.Element("kurlar")

    for currentss in cl_currencies:
        doc = ET.SubElement(root, "kur")
        doc_2 = ET.SubElement(doc, "title").text = currentss.name
        for data in currentss.all_data:
            # C# use "," operator for float percenteage part
            ET.SubElement(doc, "price").text = str("%.4f" % data).replace(".", ",")
    tree = ET.ElementTree(root)
    tree.write("currencies.xml")


def sort_and_add_indis(cl_currencies):
    cl_currencies = sorted(cl_currencies, key=lambda currencie: currencie.corelation, reverse=True)
    for count, currencie in enumerate(cl_currencies):
        currencie.order = count + 1
    return cl_currencies


class Generation:
    def __init__(self, money=0, population=None, gen_size=3, population_size=10):
        self.population_size = population_size
        self.population = []
        # If the population is defined, synchronize.
        if population is not None:
            self.population = population
            self.population_size = len(population)
        # Else create new population
        elif money != 0:
            for gens in range(0, population_size):
                tmp_chromozome = Chromozome()
                for tmp_g in range(0, gen_size):
                    tmp_chromozome.add_gen(random.randint(0, money * 3))
                self.population.append(tmp_chromozome)
                # tmp_chromozome.print_gen()
                del tmp_chromozome
        else:
            # New generation created and should do nothing...
            pass

    def get_population(self):
        return self.population

    def create_fx_values(self, currencies):
        best_currencie_values = []
        for currencie in currencies:
            # Get all currencies last values
            best_currencie_values.append(currencie.all_data.pop())

        for chromozome in self.population:
            tmp_fi = 0
            #print "fi degeri atamasi basliyor", tmp_fi
            for gen, best_value in zip(chromozome.gens, best_currencie_values):
                tmp_fi += tmp_fi + gen * best_value
                #print "gen: %s currencie: %s sonuc %s" % (gen, best_value, tmp_fi)
            #print "bir genin sonucu atandi:", tmp_fi
            chromozome.fi = tmp_fi


class Chromozome:
    def __init__(self):
        self.gens = []
        self.fi = None
        self.fi_distance = 0.0
        self.fitness_value = 0.0
        self.choose_possibility = 0.0
        self.gen_mutation_rate = []

    def add_gen(self, gen):
        self.gens.append(gen)

    def return_gen(self):
        return self.gens

    def print_gen(self):
        for gen in self.gens:
            print gen,
        print "\n"


# For roulette method
def assign_fitness_values(chromozomes, money):
    for person in chromozomes:
        person.fi_distance = abs(person.fi-money)
    chromozomes.sort(key=lambda x: x.fi_distance, reverse=False)
    chromozome_length = len(chromozomes)
    for order, chromozome in enumerate(chromozomes):
        fitness_value = 1 * ((chromozome_length - (order+1)) / (chromozome_length - 1))
        chromozome.fitness_value = float(format(fitness_value, '.3f'))
    return chromozomes


def roulette(population, elite_person, start_money):
    total_fx = 0.0
    choose_random = []
    after_generation = Generation()
    for tmp in range(0, len(population)):
        choose_random.append(float(format(random.uniform(0, 1), '.3f')))
    for tmp_chromozome in population:
        total_fx += tmp_chromozome.fitness_value
    for tmp_c in population:
        tmp_c.choose_possibility = tmp_c.fitness_value / total_fx
    for random_value in choose_random:
        tmp_roulette = 0
        piece = 0
        tmp_person = Chromozome()
        tmp_roulette += population[piece].choose_possibility
        piece += 1
        while random_value > tmp_roulette:
            tmp_roulette += population[piece].choose_possibility
            piece += 1
        tmp_person.gens = population[piece-1].gens
        tmp_person.fi = population[piece-1].fi
        after_generation.population.append(tmp_person)
        del tmp_person

    # Add elite person in after generation
    after_generation.population.sort(key=lambda x: x.fi, reverse=False)
    first_person_fi = after_generation.population[0].fi
    last_person_fi = after_generation.population[-1].fi
    first_person_distance = abs(first_person_fi - start_money)
    last_person_distance = abs(last_person_fi - start_money)
    if first_person_distance > last_person_distance:
        after_generation.population[0] = elite_person
    else:
        after_generation.population[-1] = elite_person
    return after_generation


def cross(generation):
    # all persons gen lenth equal the other...
    gen_size = len(generation.population[0].gens)
    # Create new instance Generation class and copy cross_generation variable...
    cross_generation = copy.deepcopy(generation)
    cut_off_counter = 0
    chromosome_size = len(generation.population)
    cut_off_point = int(chromosome_size/2)
    for order, person in enumerate(cross_generation.population):
        if order < gen_size / 2:
            person.gens[1] = generation.population[cut_off_point - 1 + order].gens[1]
            person.gens[2] = generation.population[cut_off_point - 1 + order].gens[2]
        else:
            person.gens[1] = generation.population[cut_off_counter].gens[1]
            person.gens[2] = generation.population[cut_off_counter].gens[2]
            cut_off_counter += 1
    return cross_generation


def mutation(generation, mutation_possibility):
    # Assign mutation change
    for person in generation.population:
        #print "genler:"
        #print person.gens
        for gen_mutation in person.gens:
            person.gen_mutation_rate.append(float(format(random.uniform(0, 1), '.3f')))
            #print "gen:", gen_mutation
        #print "bir gen bitti"
    #for person in generation.population:
        #print "bir genin mutasyon oranlari:"
        #print person.gen_mutation_rate
    # Check and mutate gen
    for person in generation.population:
        for key, gen_possibility in enumerate(person.gen_mutation_rate):
            # print "genin sirasi: %s, gen degeri: %s" % (key, gen_possibility)
            if gen_possibility <= mutation_possibility:
                person.gens[key] += 20.0
                if(key == 2):
                    break
    return generation


def find_best_person(generation, money):
    # generation.population.sort(key=lambda x: x.fi, reverse=False)
    best_person = None
    tmp_distance = abs(money-generation.population[-1].fi_distance)
    for person in generation.population:
        if person.fi_distance < tmp_distance:
            tmp_distance = person.fi_distance
            best_person = copy.deepcopy(person)
    return best_person


def genetic_loop(start_money, cl_sorted_currencies, mutation_possibility, cycle_size):
    all_generation = []
    tmp_generation = Generation(money=start_money)
    tmp_generation.create_fx_values(cl_sorted_currencies)
    tmp_generation.population = assign_fitness_values(tmp_generation.population, start_money)
    all_generation.append(tmp_generation)
    for tmp in range(0, cycle_size):
        elite_person = tmp_generation.population[0]
        tmp_generation.population = assign_fitness_values(tmp_generation.population,start_money)
        tmp_after_generation = copy.deepcopy(roulette(tmp_generation.population, elite_person, start_money))
        tmp_after_generation = cross(tmp_after_generation)
        tmp_after_generation = mutation(tmp_after_generation, mutation_possibility)
        all_generation.append(tmp_after_generation)
        tmp_generation = copy.deepcopy(tmp_after_generation)
    for order, generation in enumerate(all_generation):
        print "%s . jenerasyon birey amac fonk degerleri:" % str(order +1)
        for person in generation.population:
            print person.fi
        print "%s . jenerasyon birey genleri:" % str(order +1)
        for person in generation.population:
            print person.gens
    best_person = find_best_person(all_generation.pop(),start_money)
    return best_person