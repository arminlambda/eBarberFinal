import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ScrollView,
  Alert,
} from 'react-native';
import { getLokacije, adminCreateTermin } from '../services/api';

export default function AdminCreateTerminScreen({ navigation }) {
  const [lokacije, setLokacije] = useState([]);
  const [selectedLokacija, setSelectedLokacija] = useState(null);
  const [datum, setDatum] = useState('');
  const [zacetekUra, setZacetekUra] = useState('');
  const [konecUra, setKonecUra] = useState('');
  const [maxUporabnikov, setMaxUporabnikov] = useState('5');
  const [opis, setOpis] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadLokacije();
  }, []);

  const loadLokacije = async () => {
    try {
      const data = await getLokacije();
      setLokacije(data);
      if (data.length > 0) setSelectedLokacija(data[0].id);
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče naložiti lokacij');
    }
  };

  const handleCreate = async () => {
    if (!datum || !zacetekUra || !konecUra) {
      Alert.alert('Napaka', 'Izpolni vse podatke');
      return;
    }

    const zacetek = `${datum}T${zacetekUra}:00`;
    const konec = `${datum}T${konecUra}:00`;

    try {
      setLoading(true);
      await adminCreateTermin(
        zacetek,
        konec,
        selectedLokacija,
        parseInt(maxUporabnikov),
        opis
      );
      Alert.alert('Uspeh', 'Termin ustvarjen!', [
        { text: 'OK', onPress: () => navigation.goBack() },
      ]);
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče ustvariti termina');
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Dodaj okvirni termin</Text>

      <Text style={styles.label}>Lokacija</Text>
      {lokacije.map((lok) => (
        <TouchableOpacity
          key={lok.id}
          style={[
            styles.locationItem,
            selectedLokacija === lok.id && styles.locationItemSelected,
          ]}
          onPress={() => setSelectedLokacija(lok.id)}
        >
          <Text
            style={[
              styles.locationText,
              selectedLokacija === lok.id && styles.locationTextSelected,
            ]}
          >
            {lok.naslov}, {lok.mesto}
          </Text>
        </TouchableOpacity>
      ))}

      <Text style={styles.label}>Datum (YYYY-MM-DD)</Text>
      <TextInput
        style={styles.input}
        placeholder="2026-01-20"
        value={datum}
        onChangeText={setDatum}
      />

      <Text style={styles.label}>Začetek (HH:MM)</Text>
      <TextInput
        style={styles.input}
        placeholder="17:00"
        value={zacetekUra}
        onChangeText={setZacetekUra}
      />

      <Text style={styles.label}>Konec (HH:MM)</Text>
      <TextInput
        style={styles.input}
        placeholder="20:00"
        value={konecUra}
        onChangeText={setKonecUra}
      />

      <Text style={styles.label}>Max uporabnikov</Text>
      <TextInput
        style={styles.input}
        placeholder="5"
        value={maxUporabnikov}
        onChangeText={setMaxUporabnikov}
        keyboardType="numeric"
      />

      <Text style={styles.label}>Opis (opcijsko)</Text>
      <TextInput
        style={[styles.input, styles.textArea]}
        placeholder="Opis termina..."
        value={opis}
        onChangeText={setOpis}
        multiline
        numberOfLines={4}
      />

      <TouchableOpacity
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleCreate}
        disabled={loading}
      >
        <Text style={styles.buttonText}>
          {loading ? 'Ustvarjam...' : 'Ustvari termin'}
        </Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  content: {
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 8,
    color: '#333',
  },
  input: {
    height: 50,
    borderColor: '#ddd',
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 15,
    marginBottom: 20,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  textArea: {
    height: 100,
    paddingTop: 15,
    textAlignVertical: 'top',
  },
  locationItem: {
    padding: 15,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
    marginBottom: 10,
    backgroundColor: '#fff',
  },
  locationItemSelected: {
    backgroundColor: '#007AFF',
    borderColor: '#007AFF',
  },
  locationText: {
    fontSize: 16,
    color: '#333',
  },
  locationTextSelected: {
    color: '#fff',
    fontWeight: 'bold',
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
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