import random
konusmak_isteme = []
for tmp in range(0, 10):
    konusmak_isteme.append(random.uniform(0, 1))

for icerik in konusmak_isteme:
    print float(format(random.uniform(0, 1), '.2f'))