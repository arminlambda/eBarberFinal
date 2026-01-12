import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  RefreshControl,
  Alert,
} from 'react-native';
import { getOkvirniTermini, prijaviSeNaTermin } from '../services/api';

export default function OkvirniTerminiScreen({ navigation }) {
  const [termini, setTermini] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadTermini();
  }, []);

  const loadTermini = async () => {
    try {
      setLoading(true);
      const data = await getOkvirniTermini();
      setTermini(data);
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti terminov');
    } finally {
      setLoading(false);
    }
  };

  const handlePrijava = async (terminId) => {
    Alert.alert(
      'Prijava na termin',
      'Želite se prijaviti na ta termin?',
      [
        { text: 'Prekliči', style: 'cancel' },
        {
          text: 'Prijavi se',
          onPress: async () => {
            try {
              await prijaviSeNaTermin(terminId, '');
              Alert.alert('Uspešno', 'Uspešno ste se prijavili na termin!');
              loadTermini();
            } catch (error) {
              const msg = error.response?.data?.message || 'Napaka pri prijavi';
              Alert.alert('Napaka', msg);
            }
          },
        },
      ]
    );
  };

  const renderTermin = ({ item }) => (
    <View style={styles.card}>
      <View style={styles.cardHeader}>
        <Text style={styles.lokacija}>
          📍 {item.lokacija.naslov}, {item.lokacija.mesto}
        </Text>
        <View style={[styles.badge, item.jePolno && styles.badgePolno]}>
          <Text style={styles.badgeText}>
            {item.steviloPrijav}/{item.maksimalnoUporabnikov}
          </Text>
        </View>
      </View>

      <Text style={styles.datum}>
        {new Date(item.zacetekCasa).toLocaleDateString('sl-SI', {
          weekday: 'long',
          day: 'numeric',
          month: 'long',
          year: 'numeric',
        })}
      </Text>

      <Text style={styles.cas}>
        🕐 {new Date(item.zacetekCasa).toLocaleTimeString('sl-SI', {
          hour: '2-digit',
          minute: '2-digit',
        })}{' '}
        -{' '}
        {new Date(item.konecCasa).toLocaleTimeString('sl-SI', {
          hour: '2-digit',
          minute: '2-digit',
        })}
      </Text>

      {item.opis && <Text style={styles.opis}>{item.opis}</Text>}

      <TouchableOpacity
        style={[styles.button, item.jePolno && styles.buttonDisabled]}
        onPress={() => handlePrijava(item.id)}
        disabled={item.jePolno}
      >
        <Text style={styles.buttonText}>
          {item.jePolno ? 'Poln' : 'Prijavi se'}
        </Text>
      </TouchableOpacity>
    </View>
  );

  return (
    <View style={styles.container}>
      {termini.length === 0 ? (
        <View style={styles.emptyContainer}>
          <Text style={styles.emptyText}>Ni razpoložljivih terminov</Text>
        </View>
      ) : (
        <FlatList
          data={termini}
          renderItem={renderTermin}
          keyExtractor={(item) => item.id.toString()}
          refreshControl={
            <RefreshControl refreshing={loading} onRefresh={loadTermini} />
          }
          contentContainerStyle={styles.listContent}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  listContent: {
    padding: 16,
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#666',
  },
  card: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 12,
  },
  lokacija: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
    flex: 1,
  },
  badge: {
    backgroundColor: '#34C759',
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 12,
  },
  badgePolno: {
    backgroundColor: '#FF3B30',
  },
  badgeText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: 'bold',
  },
  datum: {
    fontSize: 15,
    color: '#666',
    marginBottom: 8,
  },
  cas: {
    fontSize: 15,
    color: '#007AFF',
    fontWeight: '600',
    marginBottom: 8,
  },
  opis: {
    fontSize: 14,
    color: '#666',
    marginBottom: 12,
    fontStyle: 'italic',
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 8,
  },
  buttonDisabled: {
    backgroundColor: '#ccc',
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
});