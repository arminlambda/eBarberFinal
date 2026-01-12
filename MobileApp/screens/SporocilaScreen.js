import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  Alert,
  ScrollView,
} from 'react-native';
import { sendMessage } from '../services/api';

export default function SporocilaScreen() {
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSend = async () => {
    if (!message.trim()) {
      Alert.alert('Napaka', 'Vnesi sporočilo');
      return;
    }

    try {
      setLoading(true);
      await sendMessage(message);
      Alert.alert('Uspeh', 'Sporočilo poslano frizerju!');
      setMessage('');
    } catch (error) {
      Alert.alert('Napaka', 'Ni mogoče poslati sporočila');
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>💬 Sporočilo frizerju</Text>
      <Text style={styles.subtitle}>Pošlji vprašanje ali prošnjo</Text>

      <TextInput
        style={styles.input}
        placeholder="Vaše sporočilo..."
        value={message}
        onChangeText={setMessage}
        multiline
        numberOfLines={8}
        textAlignVertical="top"
      />

      <TouchableOpacity
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleSend}
        disabled={loading}
      >
        <Text style={styles.buttonText}>
          {loading ? 'Pošiljam...' : 'Pošlji sporočilo'}
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
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 14,
    color: '#666',
    marginBottom: 20,
  },
  input: {
    backgroundColor: '#fff',
    borderRadius: 8,
    padding: 15,
    fontSize: 16,
    minHeight: 150,
    borderWidth: 1,
    borderColor: '#ddd',
    marginBottom: 20,
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
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